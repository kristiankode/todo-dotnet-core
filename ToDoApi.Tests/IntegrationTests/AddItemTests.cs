using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ToDoApi.Models;
using Xunit;

namespace ToDoApi.Tests.IntegrationTests
{
    public class AddItemTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;

        public AddItemTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client; 
        }

        [Fact]
        public async Task CreateTodoList_should_be_persisted()
        {
            var todoList = new Todo {Name = "My list", TodoItems = new List<TodoItem>()};

            var reqBody = JsonHelper.ConvertObjectToStringContent(todoList);
            var response = await _client.PostAsync("/todos", reqBody);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var createdTodo = JsonHelper.ConvertStringToObject<Todo>(responseBody);

            Assert.Equal(createdTodo.Name, "My list");

            var allTodos = await GetAllTodos();

            var myList = allTodos.FirstOrDefault(todo => todo.Name == "My list");

            Assert.NotNull(myList);
            Assert.True(myList.Id > 0, "New todo should be assigned an ID.");
        }

        [Fact]
        public async Task Added_item_should_be_persisted()
        {
            var todos = await GetAllTodos();

            var existingId = todos.First().Id; // the default 

            var newItem = new TodoItem
            {
                IsComplete = false,
                Name = "Test adding",
                TodoId = existingId
            };

            var todoUri = $"todos/{existingId}";

            var reqBody = JsonHelper.ConvertObjectToStringContent(newItem);
            var response = await _client.PostAsync(todoUri, reqBody);
            response.EnsureSuccessStatusCode();

            var allTodos = await GetAllTodos();
            var refetched = allTodos.FirstOrDefault(todo => todo.Id == existingId);

            Assert.NotNull(refetched);
            Assert.NotEmpty(refetched.TodoItems);
        }

        private async Task<List<Todo>> GetAllTodos()
        {
            // Get all todolists in the database
            var allLists = await _client.GetAsync("/todos");

            // not strictly necessary, but descriptive errors are useful.
            allLists.EnsureSuccessStatusCode();

            var responseBody = await allLists.Content.ReadAsStringAsync();
            var todos = JsonHelper.ConvertStringToObject<List<Todo>>(responseBody);
            return todos;
        }
    }
}
