using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class MerchantTypeDatabase
{
    public static Dictionary<MerchantType, MerchantTypeRef> database { get; private set; }

    public static void CreateDatabase()
    {
        // Load scriptable objects from file

        // Create item ref database
        MerchantTypeRef[] merchantTypeRef =
            Resources.LoadAll("ScriptableObjects/MerchantTypes",
            typeof(MerchantTypeRef))
            .Cast<MerchantTypeRef>().ToArray();

        database = new Dictionary<MerchantType, MerchantTypeRef>();
        // Create the database
        for (int i = 0; i < merchantTypeRef.Length; i++)
        {
            database.Add(
                merchantTypeRef[i].merchantType,
                merchantTypeRef[i]);
        }
    }

    public static MerchantTypeRef GetMerchantTypeRef(MerchantType mt)
    {
        if (database.TryGetValue(mt, out MerchantTypeRef merchantTypeRef))
        {
            return merchantTypeRef;
        }

        return null;
    }
}
