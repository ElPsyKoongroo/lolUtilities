using System.Linq.Expressions;

namespace LeagueUtilities.DB;
using LiteDB;


// GetCollection<T>(nombreCol)
// SaveCollection

public class DBConnection
{
    public static class CollectionName
    {
        public static string PBProfiles = "PBProfiles"; 
    }

    private LiteDatabase db;

    public DBConnection(string path)
    {
        db = new LiteDatabase(path);
    }

    public IEnumerable<T> GetCollection<T>(string collectionName)
    {
        return db.GetCollection<T>(collectionName).FindAll();
    }

    public void SaveCollection<T>(string collectionName ,IEnumerable<T> data)
    {
        var col = db.GetCollection<T>(collectionName);
        col.DeleteAll();
        col.Insert(data);
    }
    
    public void UpdateEntry<T>(string collectionName, T data)
    {
        var col = db.GetCollection<T>(collectionName);
        col.Update(data);
    }
    
    public void InsertEntry<T>(string collectionName, T data)
    {
        var col = db.GetCollection<T>(collectionName);
        col.Insert(data);
    }
    
    public void UpsertEntry<T>(string collectionName, T data)
    {
        var col = db.GetCollection<T>(collectionName);
        col.Upsert(data);
    }

    public void DeleteEntry<T>(string collectionName, ObjectId objID)
    {
        var col = db.GetCollection<T>(collectionName);
        col.Delete(objID);
    }
    
    public T? FindEntryById<T>(string collectionName, ObjectId objID)
    {
        var col = db.GetCollection<T>(collectionName);
        return col.FindById(objID);
    }
    
    public IEnumerable<T> FindEntryByProperty<T>(string collectionName, Expression<Func<T, bool>> func)
    {
        var col = db.GetCollection<T>(collectionName);
        return col.Find(func);
    }
    
    ~DBConnection()
    {
        db.Dispose();
    }




}