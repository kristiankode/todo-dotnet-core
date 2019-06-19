using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDoApi.Controllers;
using ToDoApi.Models;
using ToDoApi.Repository;
using Xunit;

namespace ToDoApi.Tests.UnitTests.Controllers
{
    
    public class RemoveItemTests
    {
        private TodoController _sut;
        private Mock<ITodoRepository> _repo; 
        public RemoveItemTests()
        {
            _repo = new Mock<ITodoRepository>();
            _sut = new TodoController(_repo.Object);
        }

        [Theory]
        [InlineData(123)]
        [InlineData(213)]
        [InlineData(89234823982)]
        public void DeleteItem_should_call_repo_with_correct_id(long id)
        {
            var item = new TodoItem {Id = id, Name = "test"};
            _repo.Setup(r => r.FindItem(id)).Returns(item);
            
            var result = _sut.DeleteItem(id);

            Assert.IsType<NoContentResult>(result);
            _repo.Verify(r => r.RemoveItem(id), Times.Once);
        }


        [Theory]
        [InlineData(123)]
        [InlineData(213)]
        [InlineData(89234823982)]
        public void DeleteItem_should_not_delete_non_existing_items(long id)
        {
            _repo.Setup(r => r.FindItem(id)).Returns((TodoItem) null);

            var result = _sut.DeleteItem(id);

            _repo.Verify(r => r.RemoveItem(id), Times.Never);
        }

        [Theory]
        [InlineData(123)]
        [InlineData(213)]
        [InlineData(89234823982)]
        public void DeleteItem_should_return_NoContent_given_existing_item(long id)
        {
            var item = new TodoItem {Id = id, Name = "test"};
            _repo.Setup(r => r.FindItem(id)).Returns(item);
            
            var result = _sut.DeleteItem(id);

            Assert.IsType<NoContentResult>(result);
            _repo.Verify(r => r.RemoveItem(id), Times.Once);
        }

        [Theory]
        [InlineData(9324)]
        [InlineData(42324)]
        [InlineData(12342)]
        public void DeleteItem_should_return_NotFound_given_non_existing_item(long id)
        {
            _repo.Setup(r => r.FindItem(id)).Returns((TodoItem) null);

            var result = _sut.DeleteItem(id);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
