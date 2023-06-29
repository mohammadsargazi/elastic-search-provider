using Microsoft.Extensions.Localization;
using Nest;
using Provider.Results;

namespace Provider.Extensions;

public static class ToResultModel
{

    #region Insert
    public static InsertResult<T> ToInsertResult<T>(this IndexResponse response, T entity, IStringLocalizer localizer)
    {
        var message = response.Result == Result.Created ? localizer["EntityInserted"] : localizer["EntityNotInserted"];
        return new InsertResult<T>(entity, response.IsValid ? 1 : 0, message);
    }

    public static InsertResult<T> ToInsertResult<T>(this T entity, bool inserted, IStringLocalizer localizer)
    {
        var message = inserted ? localizer["EntityInserted"] : localizer["EntityNotInserted"];
        return new InsertResult<T>(entity, inserted ? 1 : 0, message);
    }
    #endregion

    #region Update
    public static UpdatedResult<T> ToUpdateResult<T>(this T entity, Result result, IStringLocalizer localizer)
    {
        var updated = result == Result.Updated;
        var message = updated ? localizer["EntityUpdated"] : localizer["EntityNotUpdated"];
        return new UpdatedResult<T>(entity, updated ? 1 : 0, message);
    }
    public static UpdatedResult<T> ToUpdateResult<T>(this T entity, bool updated, IStringLocalizer localizer)
    {
        var message = updated ? localizer["EntityUpdated"] : localizer["EntityNotUpdated"];
        return new UpdatedResult<T>(entity, updated ? 1 : 0, message);
    }
    #endregion

}
