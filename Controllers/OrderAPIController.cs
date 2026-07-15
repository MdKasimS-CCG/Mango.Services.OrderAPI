using AutoMapper;

using Mango.MessageBus;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IMessageProducer _messageBus;

        /// <summary>
        /// Dont Make it private readonly, else it will be null.
        /// </summary>
        private IProductService _productService;
        private IConfiguration _configuration;
        private readonly ResponseDto _response;

        public OrderAPIController(AppDbContext db, IMapper mapper
                                , IProductService productService
                                , IMessageProducer messageBus
                                , IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _messageBus = messageBus;
            _productService = productService;
            _configuration = configuration;
            _response = new ResponseDto();
            _configuration = configuration;
        }

        //TODO: Since it is with parameter, path in OrderService of Web, is different in Post of ProductService
        [HttpPost("CreateOrder")]
        [Authorize]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                //TODO: Checkout this what it is in detail .Entity
                //TODO: Check whether await works here or not. Earlier it was not working or might be timing difference due to async-await
                OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDto.Id = orderCreated.Id;

                _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        //[HttpPost("CreateStripeSession")]
        //[Authorize]
        //public async Task<ResponseDto> CreateStripeSession([FromBody] PayementRequestDto paymentRequestDto)
        //{
        //    try
        //    {
        //        var options = new SessionCreateOptions
        //        {
        //            SuccessUrl = paymentRequestDto.ApprovedUrl,
        //            LineItems = new List<SessionLineItemOptions>()
        //            {
        //                new SessionLineItemOptions{
        //                    Price = "price_12345",
        //                    Quantity = 2,
        //                }
        //            },
        //            Mode = "payment",
        //            CancelUrl = paymentRequestDto.CancelUrl,
        //        };
        //        var service = new SessionService();
        //        service.Create(options);
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = ex.Message;
        //    }

        //    return _response;
        //}


    }
}
