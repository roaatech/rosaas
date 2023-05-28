using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Common.Models.ResponseMessages;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Education.API.Models.Common.Responses;

namespace Roaa.Rosas.Common.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        public ResponseMetadata Message { get; set; } = new ResponseMetadata();

        protected IActionResult EmptyResult()
        {
            return EmptyResult(Message);
        }
        protected IActionResult EmptyResult(Result result)
        {
            if (!result.Success)
            {
                return InvalidRequest(result.Messages);
            }

            return EmptyResult();
        }
        protected IActionResult EmptyResult<T>(Result<T> result)
        {
            if (!result.Success)
            {
                return InvalidRequest(result.Messages);
            }

            return EmptyResult();
        }
        protected IActionResult EmptyResult(ResponseMetadata message)
        {
            if (message.Success)
            {
                return Ok(new ResponseResult
                {
                    Metadata = message
                });
            }
            return InvalidRequest(message);
        }

        protected IActionResult ItemResult<T>(T data)
        {
            return ItemResult(Message, data);
        }
        protected IActionResult ItemResult<T>(Result<T> result)
        {
            if (!result.Success)
            {
                return InvalidRequest(result.Messages);
            }

            return ItemResult(Message, result.Data);
        }

        protected IActionResult ItemResult<T>(ResponseMetadata message, T data)
        {
            if (message.Success)
            {
                return Ok(new ResponseItemResult<T>
                {
                    Metadata = message,
                    Data = data
                });
            }
            return InvalidRequest(message);
        }

        protected IActionResult ListResult<T>(Result<List<T>> result)
        {
            if (!result.Success)
            {
                return InvalidRequest(result.Messages);
            }

            return ListResult(result.Data);
        }

        protected IActionResult ListResult<T>(List<T> data)
        {
            return ListResult(Message, data);
        }

        protected IActionResult ListResult<T>(ResponseMetadata message, List<T> data)
        {
            if (message.Success)
            {
                return Ok(new ResponseListResult<T>
                {
                    Metadata = message,
                    Data = data
                });
            }
            return InvalidRequest(message);
        }

        protected IActionResult PaginatedResult<T>(IEnumerable<T> data, int totalRows)
        {
            return PaginatedResult(Message, data, totalRows);
        }


        protected IActionResult PaginatedResult<T>(PaginatedResult<T> result)
        {
            if (!result.Success)
            {
                return InvalidRequest(result.Messages);
            }

            return PaginatedResult(result.Data, result.TotalCount);
        }

        protected IActionResult PaginatedResult<T>(ResponseMetadata message, IEnumerable<T> data, int totalRows)
        {
            if (message.Success)
            {
                return Ok(new ResponsePaginatedResult<T>
                {
                    Metadata = message,
                    Data = new ResponsePaginatedModel<T>
                    {
                        Items = data,
                        TotalCount = totalRows
                    }

                });
            }
            return InvalidRequest(message);
        }

        protected IActionResult InvalidRequest()
        {
            return InvalidRequest(Message);
        }

        protected IActionResult InvalidRequest(ResponseMetadata message)
        {
            return BadRequest(new ResponseResult
            {
                Metadata = message
            });
        }

        protected IActionResult InvalidRequest(List<MessageDetail> messagesDetails)
        {
            return InvalidRequest(new ResponseMetadata
            {
                Errors = messagesDetails
            });
        }
        protected IActionResult InvalidRequest(params MessageDetail[] messagesDetails)
        {
            return InvalidRequest(new ResponseMetadata
            {
                Errors = messagesDetails.ToList()
            });
        }
        protected IActionResult ForbiddenRequest(Enum sysCode, string message)
        {
            Message.Errors.Add(MessageDetail.Error(message, sysCode));

            return StatusCode(StatusCodes.Status403Forbidden, new ResponseResult
            {
                Metadata = Message
            });
        }

        [NonAction]
        // TODO middleware
        protected bool CheckLocale()
        {
            //todo Alsaadi add middleware and reject any request does not has language
            var result = HttpContext.Request.Headers.ContainsKey("Accept-Language");
            if (!result)
            {
                Message.AddError(CommonErrorKeys.AcceptLocaleRequired, Localization.LanguageEnum.en);
            }

            return result;
        }

        [NonAction]
        // TODO filter
        protected bool CheckClientId(string officialClientId)
        {
            var result = HttpContext.Request.Headers.ContainsKey("Client-Id");
            if (result)
            {
                var clientId = HttpContext.Request.Headers.FirstOrDefault(h => "Client-Id".Equals(h.Key, StringComparison.OrdinalIgnoreCase)).Value.ToString();

                if (!clientId.Equals(officialClientId, StringComparison.OrdinalIgnoreCase))
                {
                    result = false;
                    Message.AddError(CommonErrorKeys.UnAuthorizedAction, Localization.LanguageEnum.en);
                }

            }
            else
            {
                Message.AddError(CommonErrorKeys.ClientIdRequired, Localization.LanguageEnum.en);
            }

            return result;
        }
    }
}
