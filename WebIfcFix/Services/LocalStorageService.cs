using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace WebIfcFix.Services;
public interface ILocalStorageService
{
    Task SetItemAsync<T>(string key, T value);
    Task<T?> GetItemAsync<T>(string key);
    Task RemoveItemAsync(string key);
}
public class LocalStorageService(
    IJSRuntime _jsRuntime,
    IJsonConvertService _json
    ): ILocalStorageService
{
    public async Task SetItemAsync<T>(string key, T value)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, _json.SerializeObject(value));
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        string storedStrValue = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        if (String.IsNullOrEmpty(storedStrValue))
        {
            return default;
        }
        else
        {
            return _json.DeserializeObject<T>(storedStrValue);
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}
