using api.Dtos.Comment;
using api.Models;


namespace api.Mappers{
    public static class CommentMapper
    {
        
        public static CommentDto ToCommentDto(this Comment commentModel)
        {
            return new CommentDto{
                 Id = commentModel.Id,
                 Title = commentModel.Title,
                 Content = commentModel.Content,
                 CreatedOn = commentModel.CreatedOn,
                 StockId = commentModel.StockId

            };
        }


        // used in create api in commentscontroller
         public static Comment ToCommentFromCreate(this CreateCommentDto commentDto, int stockId)
        {
            return new Comment{
                 Title = commentDto.Title,
                 Content = commentDto.Content,
                 StockId = stockId
            };
        }

 // used in update api in commentscontroller

          public static Comment ToCommentFromUpdate(this UpdateCommentRequestDto commentDto, int stockId)
        {
            return new Comment
            {
                Title = commentDto.Title,
                Content = commentDto.Content,
                StockId = stockId
            };
        }

    }
}