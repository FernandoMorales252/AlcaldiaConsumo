using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AlcaldiaFront.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        // --- Método Auxiliar para Manejo de Respuesta (CLAVE) ---
        private async Task<string> HandleResponse(HttpResponseMessage response)
        {
            // Si la respuesta es 204 No Content, no hay JSON para leer.
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return string.Empty;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Si la respuesta no fue exitosa (4xx, 5xx), leemos el contenido para obtener el mensaje de error.
            if (!response.IsSuccessStatusCode)
            {
                // Intenta parsear el error para mejor diagnóstico, si el API lo proporciona en JSON.
                // Podrías lanzar una excepción personalizada aquí para capturar este error en el Controller.
                System.Console.WriteLine($"Error HTTP {response.StatusCode}: {jsonResponse}");

                // Lanza una excepción estándar, pero ahora tenemos el error en consola para depuración.
                response.EnsureSuccessStatusCode();
            }

            return jsonResponse;
        }
        // --------------------------------------------------------

        // Obtener todos los datos de un endpoint
        public async Task<List<T>> GetAllAsync<T>(string endpoint, string token = null)
        {
            AddAuthorizationHeader(token);
            var response = await _httpClient.GetAsync(endpoint);
            var JsonResponse = await HandleResponse(response);

            // Si el JsonResponse es vacío (ej. 204), devuelve una lista vacía
            if (string.IsNullOrEmpty(JsonResponse))
            {
                return new List<T>();
            }

            return JsonSerializer.Deserialize<List<T>>(JsonResponse, _jsonOptions);
        }

        // Obtener por Id
        public async Task<T> GetByIdAsync<T>(string endpoint, int Id, string token = null)
        {
            AddAuthorizationHeader(token);
            var response = await _httpClient.GetAsync($"{endpoint}/{Id}");
            var JsonResponse = await HandleResponse(response);

            // Si el JsonResponse es vacío, lanza excepción o devuelve default
            if (string.IsNullOrEmpty(JsonResponse))
            {
                throw new HttpRequestException($"No content received for ID {Id} on endpoint {endpoint}", null, response.StatusCode);
            }

            return JsonSerializer.Deserialize<T>(JsonResponse, _jsonOptions);
        }

        // Post generico
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, string token = null)
        {
            AddAuthorizationHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(data, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            var json = await HandleResponse(response);

            if (string.IsNullOrEmpty(json))
            {
                return default(TResponse);
            }

            return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, int Id, TRequest data, string token = null)
        {
            AddAuthorizationHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(data, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{endpoint}/{Id}", content);
            var JsonResponse = await HandleResponse(response);

            // Manejo de 204 No Content (JsonResponse será string.Empty)
            if (string.IsNullOrEmpty(JsonResponse))
            {
                return default(TResponse);
            }

            // Deserialización normal (para respuestas 200 OK con cuerpo)
            return JsonSerializer.Deserialize<TResponse>(JsonResponse, _jsonOptions);
        }

        // Delete generico 
        public async Task<bool> DeleteAsync(string endpoint, int Id, string token = null)
        {
            AddAuthorizationHeader(token);
            var response = await _httpClient.DeleteAsync($"{endpoint}/{Id}");

            // Llama a HandleResponse para loguear errores si existen, pero la verificación
            // de éxito principal para un DELETE simple sigue siendo IsSuccessStatusCode.
            await HandleResponse(response);

            return response.IsSuccessStatusCode;
        }

        // === NUEVO MÉTODO PARA DEVOLVER BYTES DE ARCHIVO (Usa el cliente configurado) ===
        public async Task<byte[]?> GetFileBytesAsync(string endpoint, string token)
        {
            AddAuthorizationHeader(token);
            Console.WriteLine($"--- API SERVICE GET BYTES: {_httpClient.BaseAddress}{endpoint} ---");

            try
            {
                // Usamos el cliente configurado de ApiService (_httpClient)
                using (var response = await _httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // ÉXITO: Devuelve los bytes directamente
                        return await response.Content.ReadAsByteArrayAsync();
                    }
                    else
                    {
                        // FALLO: Imprimimos el código de error.
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error HTTP en GetFileBytesAsync. Código: {response.StatusCode}. Contenido: {errorContent}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción en ApiService.GetFileBytesAsync: {ex.Message}");
                return null;
            }
        }
        // --------------------------------------------------

        // Agregar Authorization header
        public void AddAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Exponemos el BaseAddress para referencia, aunque no es usado en GetFileBytesAsync
        public Uri? BaseAddress => _httpClient.BaseAddress;
    }
}