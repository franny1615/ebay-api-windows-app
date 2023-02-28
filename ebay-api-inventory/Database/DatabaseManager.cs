using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ebay_api_inventory.Entities;
using Microsoft.Data.Sqlite;

namespace ebay_api_inventory.Database;

public class DatabaseManager
{
    SqliteConnection dbConnection;
    private string dbPath = Environment.CurrentDirectory + "\\DB";
    private string dbFilePath = "";

    public DatabaseManager()
    {
        if (!Directory.Exists(dbPath))
        {
            Directory.CreateDirectory(dbPath);
        }
        
        dbFilePath = dbPath + "\\ebayInventory.db";
        dbConnection = new SqliteConnection($"Data Source={dbFilePath}");
        createInventoryDataIfNotExist();
        createUserAccessTokenTable();
    }

    private void createInventoryDataIfNotExist()
    {
        using (dbConnection)
        {
            dbConnection.Open();
            var command = dbConnection.CreateCommand();
            
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS inventoryTable (
                INTERNAL_ID INTEGER PRIMARY KEY,
                TITLE TEXT,
                LISTING_TYPE TEXT,
                START_TIME TEXT,
                AVAILABLE_QTY INTEGER,
                QUANTITY INTEGER,
                BUY_IT_NOW_PRICE REAL,
                CURRENCY TEXT,
                BEST_OFFER_COUNT INTEGER,
                ITEM_ID TEXT,
                STORAGE_LOCATION TEXT
            )
            ";

            command.ExecuteNonQuery();
        }
    }

    private void createUserAccessTokenTable()
    {
        using(dbConnection)
        {
            dbConnection.Open();

            var command = dbConnection.CreateCommand();
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS userAccessTokenTable (
                INTERNAL_ID INTEGER PRIMARY KEY,
                ACCESS_TOKEN TEXT,
                EXPIRES_IN INTEGER,
                REFRESH_TOKEN TEXT,
                REFRESH_TOKEN_EXPIRES_IN INTEGER,
                TOKEN_TYPE TEXT,
                INSERTED_AT_IN_SECONDS INT
            )
            ";

            command.ExecuteNonQuery();
        }
    }

    public void insertAccessToken(UserAccessToken token)
    {
        using(dbConnection)
        {
            dbConnection.Open();

            var command = dbConnection.CreateCommand();
            command.CommandText = @"
                DELETE FROM userAccessTokenTable;
                INSERT INTO userAccessTokenTable 
                       (ACCESS_TOKEN, EXPIRES_IN, REFRESH_TOKEN, REFRESH_TOKEN_EXPIRES_IN, TOKEN_TYPE, INSERTED_AT_IN_SECONDS)
                VALUES ($accessToken, $expiresIn, $refreshToken, $refreshTokenExpiresIn, $tokenType, $insertedAtInSeconds);
            ";
            command.Parameters.AddWithValue("$accessToken", token.access_token);
            command.Parameters.AddWithValue("$expiresIn", token.expires_in);
            command.Parameters.AddWithValue("$refreshToken", token.refresh_token);
            command.Parameters.AddWithValue("$refreshTokenExpiresIn", token.refresh_token_expires_in);
            command.Parameters.AddWithValue("$tokenType", token.token_type);
            command.Parameters.AddWithValue("$insertedAtInSeconds", token.insertedAtInSeconds);

            command.ExecuteNonQuery();
        }
    }

    public UserAccessToken? getUserAccessToken()
    {
        using(dbConnection)
        {
            dbConnection.Open();

            var command = dbConnection.CreateCommand();
            command.CommandText = @"
                SELECT ACCESS_TOKEN, EXPIRES_IN, REFRESH_TOKEN, REFRESH_TOKEN_EXPIRES_IN, TOKEN_TYPE
                FROM userAccessTokenTable;
            ";

            using (var reader = command.ExecuteReader())
            {
                try
                {
                    reader.Read(); // there should always only be one user access token stored in DB.
                    var userAccessToken = new UserAccessToken();
                    userAccessToken.access_token = reader.GetString(0);
                    userAccessToken.expires_in = (int)reader.GetInt64(1);
                    userAccessToken.refresh_token = reader.GetString(2);
                    userAccessToken.refresh_token_expires_in = (int)reader.GetInt64(3);
                    userAccessToken.token_type = reader.GetString(4);

                    return userAccessToken;
                }
                catch
                {
                    return null;
                }
            }
        }
    }

    public List<eBayListing> syncInventoryWith(List<eBayListing> inventory)
    {
        var syncedInventory = new List<eBayListing>();

        for (int index = 0; index < inventory.Count; index = index + 1) 
        {
            if (listingExists(inventory[index]))
            {
                var updatedListing = updateListingInDBWith(inventory[index]);
                syncedInventory.Add(updatedListing);
            }
            else
            {
                var insertedListing = insertListing(inventory[index]);
                syncedInventory.Add(insertedListing);
            }
        }

        return syncedInventory;
    }

    public eBayListing updatedStorageLocationFor(eBayListing listing)
    {
        using(dbConnection)
        {
            dbConnection.Open();

            var command = dbConnection.CreateCommand();
            command.CommandText =
            @"
               UPDATE inventoryTable
               SET 
                   STORAGE_LOCATION = $storageLocation
               WHERE 
                   ITEM_ID = $itemId
            ";
            command.Parameters.AddWithValue("$storageLocation", listing.storageLocation);
            command.Parameters.AddWithValue("$itemId", listing.itemId);

            command.ExecuteNonQuery();
        }
        return new eBayListing();
    }

    private eBayListing insertListing(eBayListing listing)
    {
        using(dbConnection)
        {
            dbConnection.Open();

            var command = dbConnection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO inventoryTable 
                       (TITLE, LISTING_TYPE, START_TIME, AVAILABLE_QTY, QUANTITY, BUY_IT_NOW_PRICE, CURRENCY, BEST_OFFER_COUNT, ITEM_ID, STORAGE_LOCATION)
                VALUES ($title, $listingType, $startTime, $availableQty, $quantity, $buyItNowPrice, $currency, $bestOfferCount, $itemId, $storageLocation)
            ";
            command.Parameters.AddWithValue("$title", listing.title);
            command.Parameters.AddWithValue("$listingType", listing.listingType);
            command.Parameters.AddWithValue("$startTime", listing.startTime);
            command.Parameters.AddWithValue("$availableQty", listing.availableQuantity);
            command.Parameters.AddWithValue("$quantity", listing.quantity);
            command.Parameters.AddWithValue("$buyItNowPrice", listing.buyItNowPrice);
            command.Parameters.AddWithValue("$currency", listing.currencyType);
            command.Parameters.AddWithValue("$bestOfferCount", listing.bestOfferCount);
            command.Parameters.AddWithValue("$itemId", listing.itemId);
            command.Parameters.AddWithValue("$storageLocation", listing.storageLocation);

            command.ExecuteNonQuery();
        }

        return listing;
    }

    private bool listingExists(eBayListing listing)
    {
        using (dbConnection)
        {
            dbConnection.Open();

            var command = dbConnection.CreateCommand();
            command.CommandText = @"SELECT ITEM_ID FROM inventoryTable WHERE ITEM_ID = $itemId";
            command.Parameters.AddWithValue("$itemId", listing.itemId);

            using (var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    var itemId = reader.GetString(0);
                    return itemId == listing.itemId;
                }
            }
        }

        return false;
    }

    private eBayListing updateListingInDBWith(eBayListing listing)
    {
        using (dbConnection) // grab storage location if any
        {
            dbConnection.Open();

            var command = dbConnection.CreateCommand();
            command.CommandText =
             @"
                SELECT STORAGE_LOCATION
                FROM inventoryTable
                WHERE ITEM_ID = $itemId
             ";
            command.Parameters.AddWithValue("$itemId", listing.itemId);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var storageLocation = reader.GetString(0);
                    listing.storageLocation = storageLocation;
                }
            }
        }

        using (dbConnection) // update other data from network data
        {
            dbConnection.Open();

            var command = dbConnection.CreateCommand();
            command.CommandText =
            @"
               UPDATE inventoryTable
               SET TITLE = $title,
                   LISTING_TYPE = $listingType,
                   START_TIME = $startTime,
                   AVAILABLE_QTY = $availableQty,
                   QUANTITY = $quantity,
                   BUY_IT_NOW_PRICE = $buyItNowPrice,
                   CURRENCY = $currency,
                   BEST_OFFER_COUNT = $bestOfferCount
               WHERE 
                   ITEM_ID = $itemId
            ";
            command.Parameters.AddWithValue("$title", listing.title);
            command.Parameters.AddWithValue("$listingType", listing.listingType);
            command.Parameters.AddWithValue("$startTime", listing.startTime);
            command.Parameters.AddWithValue("$availableQty", listing.availableQuantity);
            command.Parameters.AddWithValue("$quantity", listing.quantity);
            command.Parameters.AddWithValue("$buyItNowPrice", listing.buyItNowPrice);
            command.Parameters.AddWithValue("$currency", listing.currencyType);
            command.Parameters.AddWithValue("$bestOfferCount", listing.bestOfferCount);
            command.Parameters.AddWithValue("$itemId", listing.itemId);

            command.ExecuteNonQuery();
        }

        return listing;
    }
}
