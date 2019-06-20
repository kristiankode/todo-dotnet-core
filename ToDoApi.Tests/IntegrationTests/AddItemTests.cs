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

            var response = await CreateTodoList(todoList);

            var responseBody = await response.Content.ReadAsStringAsync();
            var createdTodo = JsonHelper.ToObject<Todo>(responseBody);

            Assert.Equal(createdTodo.Name, "My list");

            var allTodos = await GetAllTodos();

            var myList = allTodos.FirstOrDefault(todo => todo.Name == "My list");

            Assert.NotNull(myList);
            Assert.True(myList.Id > 0, "New todo should be assigned an ID.");
        }

        private async Task<HttpResponseMessage> CreateTodoList(Todo todoList)
        {
            var reqBody = JsonHelper.ConvertObjectToStringContent(todoList);
            var response = await _client.PostAsync("/todos", reqBody);

            response.EnsureSuccessStatusCode();
            return response;
        }

        [Fact]
        public async Task Added_item_should_be_persisted()
        {
            await CreateTodoList(new Todo {Name = "Test todo", TodoItems = new List<TodoItem>()});

            var todos = await GetAllTodos();
            var existingId = todos.First().Id; 

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


        [Fact]
        public async void DeletedItem_should_be_removed()
        {
            var myTodo = new Todo
            {
                Id = 313,
                Name = "Delete",
                TodoItems = new List<TodoItem>
                {
                    new TodoItem
                    {
                        Id=3131, Name = "Delete me",
                        TodoId = 313
                    }, 
                    new TodoItem
                    {
                        Id = 3132, Name = "Don't delete me",
                        TodoId = 313
                    }
                }
            };

            await CreateTodoList(myTodo);

            var response = await _client.DeleteAsync("/todos/items/3131");
            response.EnsureSuccessStatusCode();

            var todo = await GetTodoList(313);
            Assert.True(todo.TodoItems.Any(item => item.Name == "Don't delete me"));
            Assert.False(todo.TodoItems.Any(item => item.Name == "Delete me"));
            Assert.Equal( 1, todo.TodoItems.Count);
        }


        private async Task<Todo> GetTodoList(long id)
        {
            var result = await _client.GetAsync($"/todos/{id}");
            result.EnsureSuccessStatusCode();

            var body = await result.Content.ReadAsStringAsync();
            var todo = JsonHelper.ToObject<Todo>(body);

            return todo;

        }
        private async Task<List<Todo>> GetAllTodos()
        {
            // Get all todolists in the database
            var allLists = await _client.GetAsync("/todos");

            // not strictly necessary, but descriptive errors are useful.
            allLists.EnsureSuccessStatusCode();

            var responseBody = await allLists.Content.ReadAsStringAsync();
            var todos = JsonHelper.ToObject<List<Todo>>(responseBody);
            return todos;
        }
    }
}
