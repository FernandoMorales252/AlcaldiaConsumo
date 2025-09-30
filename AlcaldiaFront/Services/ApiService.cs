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

        //Obtener todos los datos de un endpoint
        public async Task<List<T>> GetAllAsync<T>(string endpoint, string token = null)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var JsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(JsonResponse, _jsonOptions);
        }

        //Obtener por Id
        public async Task<T> GetByIdAsync<T>(string endpoint, int Id, string token = null)
        {
            var response = await _httpClient.GetAsync($"{endpoint}/{Id}");
            response.EnsureSuccessStatusCode();

            var JsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(JsonResponse, _jsonOptions);
        }

        //Post generico
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, string token = null)
        {
            var content = new StringContent(JsonSerializer.Serialize(data, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
        }



        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, int Id, TRequest data, string token = null)
        {
            // 1. Configuración y Llamada
            var content = new StringContent(JsonSerializer.Serialize(data, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{endpoint}/{Id}", content);

            // Si la respuesta no es exitosa (400, 404, 500), lanza una excepción
            // NOTA: Si quieres ver el mensaje de error del backend, deberías mejorar
            // esta sección con un try-catch que lea el contenido del error antes de EnsureSuccessStatusCode
            response.EnsureSuccessStatusCode();

            // 2. Manejo de 204 No Content (¡La Solución!)
            // Si la actualización es exitosa (código 204), no hay cuerpo JSON para leer.
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                // Retorna el valor por defecto para TResponse (ej. null para clases)
                return default(TResponse);
            }

            // 3. Deserialización normal (para respuestas 200 OK con cuerpo)
            var JsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(JsonResponse, _jsonOptions);
        }



        //Delete generico 
        public async Task<bool> DeleteAsync(string endpoint, int Id, string token = null)
        {
            var response = await _httpClient.DeleteAsync($"{endpoint}/{Id}");
            return response.IsSuccessStatusCode;
        }

        //Agregar Authorization header
        public void AddAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }



    }
}

