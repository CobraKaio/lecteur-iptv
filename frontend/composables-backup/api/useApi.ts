import { useRuntimeConfig } from 'nuxt/app';

export interface ApiResponse<T> {
  data: T | null;
  error: string | null;
  loading: boolean;
}

export function useApi() {
  const config = useRuntimeConfig();
  const baseUrl = config.public.apiBase;

  /**
   * Fonction générique pour effectuer des requêtes API
   */
  async function fetchApi<T>(
    endpoint: string, 
    options: RequestInit = {}
  ): Promise<ApiResponse<T>> {
    const url = `${baseUrl}${endpoint}`;
    const response = ref<ApiResponse<T>>({
      data: null,
      error: null,
      loading: true
    });

    try {
      const res = await fetch(url, {
        headers: {
          'Content-Type': 'application/json',
          ...options.headers
        },
        ...options
      });

      if (!res.ok) {
        throw new Error(`API error: ${res.status} ${res.statusText}`);
      }

      const data = await res.json();
      response.value = {
        data,
        error: null,
        loading: false
      };
    } catch (error) {
      console.error('API request failed:', error);
      response.value = {
        data: null,
        error: error instanceof Error ? error.message : 'Unknown error',
        loading: false
      };
    }

    return response.value;
  }

  /**
   * Fonction pour effectuer une requête GET
   */
  async function get<T>(endpoint: string, params: Record<string, string> = {}): Promise<ApiResponse<T>> {
    const queryParams = new URLSearchParams(params).toString();
    const url = queryParams ? `${endpoint}?${queryParams}` : endpoint;
    return fetchApi<T>(url, { method: 'GET' });
  }

  /**
   * Fonction pour effectuer une requête POST
   */
  async function post<T>(endpoint: string, data: any): Promise<ApiResponse<T>> {
    return fetchApi<T>(endpoint, {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }

  /**
   * Fonction pour effectuer une requête PUT
   */
  async function put<T>(endpoint: string, data: any): Promise<ApiResponse<T>> {
    return fetchApi<T>(endpoint, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  }

  /**
   * Fonction pour effectuer une requête DELETE
   */
  async function del<T>(endpoint: string): Promise<ApiResponse<T>> {
    return fetchApi<T>(endpoint, { method: 'DELETE' });
  }

  return {
    get,
    post,
    put,
    del
  };
}
