using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Wox.Plugin.MSToDo
{
    public class MSToDo : IPlugin
    {
        private static IPublicClientApplication _publicClientApp;
        private static AuthenticationResult _authResult;
        private PluginInitContext _context;
        private int _todoLength;

        private readonly TodoTask _todoTask = new TodoTask
        {
            Title = "",
            Importance = null
        };

        public void Init(PluginInitContext context)
        {
            _context = context;
        }


        public List<Result> Query(Query query)
        {
            var results = new List<Result>();
            _todoTask.Title = query.Search;
            results.Add(GetResult(query).Result);

            return results;
        }

        public async Task<Result> GetResult(Query query)
        {
            var task = query.Search;
            var client = await SignInAndInitializeGraphServiceClient(Config.Scopes);

            var lists = await client.Me.Todo.Lists
                .Request()
                .GetAsync();


            const string defaultList = "Tasks";
            var defaultTasksList = lists.Single(x => x.DisplayName == defaultList);
            var listId = defaultTasksList.Id;


            return new Result
            {
                Title = "Create new task",
                SubTitle = $"{_todoTask.Title}",
                IcoPath = "app.png",
                Action = e =>
                {
                    _todoLength = createTask(client, listId);
                    return true;
                }
            };
        }

        public int createTask(GraphServiceClient client, string listId)
        {
            _todoTask.Importance = Importance.High;

            client.Me.Todo.Lists[listId].Tasks
                .Request()
                .AddAsync(_todoTask);

            return _todoTask.Title.Length;
        }


        private static async Task<GraphServiceClient> SignInAndInitializeGraphServiceClient(string[] scopes)
        {
            var graphClient = new GraphServiceClient(Config.MSGraphURL,
                new DelegateAuthenticationProvider(async requestMessage =>
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("bearer", await SignInUserAndGetTokenUsingMSAL(scopes));
                }));

            return await Task.FromResult(graphClient);
        }

        private static async Task<string> SignInUserAndGetTokenUsingMSAL(string[] scopes)
        {
            _publicClientApp = PublicClientApplicationBuilder.Create(Config.ClientId)
                .WithRedirectUri("http://localhost")
                .Build();

            //  Configure the storage
            var cacheHelper = await CacheHelper.CreateCacheHelperAsync().ConfigureAwait(false);

            // Let the cache helper handle MSAL's cache
            cacheHelper.RegisterCache(_publicClientApp.UserTokenCache);

            var accounts = await _publicClientApp.GetAccountsAsync().ConfigureAwait(false);
            var firstAccount = accounts.FirstOrDefault();

            try
            {
                _authResult = await _publicClientApp.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                Console.WriteLine($"MsalUiRequiredException: {ex.Message}");

                _authResult = await _publicClientApp.AcquireTokenInteractive(scopes)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }

            return _authResult.AccessToken;
        }
    }
}