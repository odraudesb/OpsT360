/**
 * Configuración central de endpoints para TODO el proyecto.
 *
 * Cambia solo esta línea para todo el frontend:
 * - 'prod'      => usa servidor público (internet)
 * - 'localhost' => usa servicios en la PC local (sin internet)
 */
export const ACTIVE_API_ENV: 'prod' | 'localhost' = 'prod';

const API_BASE_URLS = {
  prod: '/api',
  localhost: '/api',
} as const;

// Nota: en desarrollo (ng serve), el host real lo define el proxy:
// - proxy.produccion.conf.json => http://38.242.225.119:3000
// - proxy.localhost.conf.json  => http://localhost:3000

const ROBOFLOW_CONFIG = {
  prod: {
    baseUrl: 'https://serverless.roboflow.com',
    workspace: 'mi-workspace-sihjw',
    workflow: 'detect-count-and-visualize-4',
    apiKey: '8OQBCU7lFbC9ogYMmbB7',
  },
  localhost: {
    // Si tienes Roboflow/inferencia local, cambia estos valores.
    baseUrl: 'http://localhost:9001',
    workspace: 'mi-workspace-sihjw',
    workflow: 'detect-count-and-visualize-4',
    apiKey: '8OQBCU7lFbC9ogYMmbB7',
  },
} as const;

export const API_BASE_URL = API_BASE_URLS[ACTIVE_API_ENV];
export const ROBOFLOW_BASE_URL = ROBOFLOW_CONFIG[ACTIVE_API_ENV].baseUrl;
export const ROBOFLOW_WORKSPACE = ROBOFLOW_CONFIG[ACTIVE_API_ENV].workspace;
export const ROBOFLOW_WORKFLOW = ROBOFLOW_CONFIG[ACTIVE_API_ENV].workflow;
export const ROBOFLOW_API_KEY = ROBOFLOW_CONFIG[ACTIVE_API_ENV].apiKey;
