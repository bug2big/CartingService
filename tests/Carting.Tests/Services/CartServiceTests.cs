using Carting.Data.Entities;
using Carting.Data.Repositories.Carts;
using Carting.Mappers;
using Carting.Models;
using Carting.Services.Carts;
using FluentAssertions;
using Moq;

namespace Carting.Services.Tests;

[TestClass]
public class CartServiceTests
{
    private Mock<ICartRepository> _cartRepository = null!;
    private Mock<CartMapper> _cartMapper = null!;
    private ICartService _cartService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _cartRepository = new Mock<ICartRepository>();
        _cartMapper = new Mock<CartMapper>();
        _cartService = new CartService(
            _cartRepository.Object, 
            _cartMapper.Object);    
    }

    [TestMethod]
    public async Task UpsertAsync_WhenCartItemModelIsValid_ShouldCallUpsert()
    {
        // Arrange 
        var cartId = Guid.NewGuid();
        var cartItemModel = new CartItemModel
        { 
            Id = Guid.NewGuid(),
            Name = "cart 1",
            Quantity = 4
        };
        
        // Act
        await _cartService.UpsertAsync(cartItemModel, cartId);

        // Assert
        _cartRepository.Verify(
            x => x.UpsertAsync(
                It.Is<Cart>(p => p.CartId == cartId
                 && p.Name == cartItemModel.Name
                 && p.Id == cartItemModel.Id
                 && p.Quantity == cartItemModel.Quantity)), Times.Once);
    }

    [TestMethod]
    public async Task GetCartItemsByCartIdAsync_WhenCartIdIsValid_ShouldReturnResult() 
    {
        // Arrange 
        var cartId = Guid.NewGuid();
        var cartItems = Enumerable.Range(1, 5)
            .Select(x => new Cart 
            { 
                CartId = cartId,
                Id = Guid.NewGuid(),
                Name = $"cart{x}",
                Quantity = x
            });

        _cartRepository.Setup(s => s.GetItemsByCartIdAsync(cartId))
            .ReturnsAsync(cartItems);

        // Act
        var result = await _cartService.GetCartItemsByCartIdAsync(cartId);
        
        // Assert
        result.Should().NotBeNullOrEmpty()
            .And.HaveCount(cartItems.Count());
    }

    [TestMethod]
    public async Task DeleteAsync_WhenValidIdsAreProvided_ShouldCall()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cartItemId = Guid.NewGuid();

        // Act
        await _cartService.DeleteAsync(cartId, cartItemId);

        // Assert
        _cartRepository.Verify(s => s.DeleteCartItemByIdAndCartIdAsync(
            It.Is<Guid>(s => s == cartId),
            It.Is<Guid>(s => s == cartItemId)), 
            Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_WhenRepositoryThrowException_ShouldThrowException()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cartItemId = Guid.NewGuid();
        var customException = new Exception("custom message");
        _cartRepository.Setup(s => s.DeleteCartItemByIdAndCartIdAsync(cartId, cartItemId))
            .ThrowsAsync(customException);

        // Act & Asser
        await _cartService.Invoking(s => s.DeleteAsync(cartId, cartItemId))
            .Should()
            .ThrowAsync<Exception>()
            .WithMessage(customException.Message);
        _cartRepository.Verify(s => s.DeleteCartItemByIdAndCartIdAsync(
            It.Is<Guid>(s => s == cartId),
            It.Is<Guid>(s => s == cartItemId)),
            Times.Once);
    }
}
