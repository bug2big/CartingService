using Carting.Identity;
using Carting.Models;
using Carting.Services.Carts;
using Microsoft.AspNetCore.Authorization;

namespace Carting.Controllers;

[Route("api/v{version:apiVersion}/carts")]
[ApiController]
[ApiVersion(1.0)]
[ApiVersion(2.0)]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(
        ICartService cartService)
    {
        _cartService = cartService;
    }

    [Authorize(Roles = $"{IdentityData.ManagerRoleClaimValue}, {IdentityData.BuyerRoleClaimValue}")]
    [HttpGet("{cartId}")]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> GetCartV1(Guid cartId)
    {
        var model = new CartModel
        {
            CartId = cartId,
            Items = await _cartService.GetCartItemsByCartIdAsync(cartId)
        };

        return Ok(model);
    }

    [Authorize(Roles = $"{IdentityData.ManagerRoleClaimValue}, {IdentityData.BuyerRoleClaimValue}")]
    [HttpGet("{cartId}")]
    [MapToApiVersion(2.0)]
    public async Task<IActionResult> GetCartV2(Guid cartId)
    {
        var model = await _cartService.GetCartItemsByCartIdAsync(cartId);
        return Ok(model);
    }

    [Authorize(Roles = IdentityData.ManagerRoleClaimValue)]
    [HttpPost("{cartId}", Name = "AddItemToCart")]
    public async Task<IActionResult> AddItemToCart(
        Guid cartId,
        [FromBody] CartItemModel cartItemModel)
    {
        await _cartService.UpsertAsync(cartItemModel, cartId);
        return Ok();
    }

    [Authorize(Roles = IdentityData.ManagerRoleClaimValue)]
    [HttpDelete("{cartId}", Name = "DeleteCartItem")]
    public async Task<IActionResult> DeleteCartItem(
        Guid cartId,
        Guid cartItemId)
    {
        await _cartService.DeleteAsync(cartId, cartItemId);
        return Ok();
    }
}
