using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using TG.Blazor.IndexedDB;
using TodosUITranslator;

namespace BlazorTodos
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddSingleton<UITranslator>();

            if (LocalData.UsingIndexedDb)
            {
                services.AddIndexedDB(dbStore =>
            {
                dbStore.DbName = "UILangDictDB";
                dbStore.Version = 1;


                dbStore.Stores.Add(new StoreSchema
                {
                    Name = "UILanguages",
                    PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = false, Unique = true }
                });

                dbStore.Stores.Add(new StoreSchema
                {
                    Name = "UILangDictGeo",
                    PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = true },
                    Indexes = new List<IndexSpec>
                    {
                        new IndexSpec{Name="native", KeyPath = "native", Auto=false},
                        new IndexSpec{Name="foreign", KeyPath = "foreign", Auto=false},
                    }
                });


                dbStore.Stores.Add(new StoreSchema
                {
                    Name = "UILangDictRus",
                    PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = true },
                    Indexes = new List<IndexSpec>
                    {
                        new IndexSpec{Name="native", KeyPath = "native", Auto=false},
                        new IndexSpec{Name="foreign", KeyPath = "foreign", Auto=false},
                    }
                });
            });
            }
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
