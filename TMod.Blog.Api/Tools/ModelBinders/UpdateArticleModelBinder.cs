using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

using System.Text.Json;

using TMod.Blog.Data.Models.DTO.Articles;

namespace TMod.Blog.Api.Tools.ModelBinders
{
    internal class UpdateArticleModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext,nameof(bindingContext));
            var form = bindingContext.HttpContext.Request.Form;
            if ( form.TryGetValue("model", out var formValue) )
            {
                if ( !string.IsNullOrWhiteSpace(formValue.ToString()) )
                {
                    UpdateArticleModel? model = JsonSerializer.Deserialize<UpdateArticleModel>(formValue.ToString());
                    if ( model is not null )
                    {
                        if(model.Meta is null )
                        {
                            bindingContext.ModelState.TryAddModelError("Meta", "元数据不能为空");
                        }
                        else if(model.Meta.Article is null )
                        {
                            bindingContext.ModelState.TryAddModelError("Meta.Article", "文章元数据不能为空");
                        }
                        else if(model.Meta.ArticleContent is null )
                        {
                            bindingContext.ModelState.TryAddModelError("Meta.ArticleContent", "文章正文元数据不能为空");
                        }
                        bindingContext.Result = ModelBindingResult.Success(model);
                    }
                    else
                    {
                        bindingContext.Result = ModelBindingResult.Failed();
                    }
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
            return Task.CompletedTask;
        }
    }

    internal class UpdateArticleModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            if(context.Metadata.ModelType == typeof(UpdateArticleModel) )
            {
                return new BinderTypeModelBinder(typeof(UpdateArticleModelBinder));
            }
            return null;
        }
    }
}
