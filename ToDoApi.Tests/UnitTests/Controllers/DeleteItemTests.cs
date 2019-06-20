using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDoApi.Controllers;
using ToDoApi.Models;
using ToDoApi.Repository;
using Xunit;

namespace ToDoApi.Tests.UnitTests.Controllers
{
    public class DeleteItemTests
    {


        [Theory]
        [InlineData(123)]
        [InlineData(4238)]
        [InlineData(8234132)]
        public void DeleteItem_should_use_repo_to_delete(long id)
        {
            var repo = new Mock<ITodoRepository>();
            repo.Setup(r => r.FindItem(id)).Returns(new TodoItem {Name = "Test"});
            var sut = new TodoController(repo.Object);

            sut.DeleteItem(id);

            repo.Verify(r => r.RemoveItem(id));
        }

        [Theory]
        [InlineData(123)]
        [InlineData(4238)]
        [InlineData(8234132)]
        public void DeleteItem_should_return_NoContent_repsonse(long id)
        {
            var repo = new Mock<ITodoRepository>();
            var sut = new TodoController(repo.Object);

            var result = sut.DeleteItem(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Theory]
        [InlineData(123)]
        [InlineData(4238)]
        [InlineData(8234132)]
        public void Should_not_delete_non_existing_items(long id)
        {
            var repo = new Mock<ITodoRepository>();
            repo.Setup(r => r.FindItem(id))
                .Returns((TodoItem) null);
            
            var sut = new TodoController(repo.Object);

            sut.DeleteItem(id);
            repo.Verify(r => r.RemoveItem(id), Times.Never);
        }

    }
}
