using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TG.Blazor.IndexedDB;

namespace BlazorTodos
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            MyBootstrap(builder);


            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();

            await builder.Build().RunAsync();
        }





        public static void MyBootstrap(WebAssemblyHostBuilder builder)
        {

            //services.AddSingleton<UITranslator>();

            if (LocalData.UsingIndexedDb)
            {
                builder.Services.AddOptions().AddIndexedDB(dbStore =>
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
    }
}
