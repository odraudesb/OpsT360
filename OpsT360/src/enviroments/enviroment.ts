import {
  API_BASE_URL,
  ROBOFLOW_API_KEY,
  ROBOFLOW_BASE_URL,
  ROBOFLOW_WORKFLOW,
  ROBOFLOW_WORKSPACE,
} from './api-endpoints';

export const environment = {
  production: false,
  apiBaseUrl: API_BASE_URL,
  roboflow: {
    baseUrl: ROBOFLOW_BASE_URL,
    workspace: ROBOFLOW_WORKSPACE,
    workflow: ROBOFLOW_WORKFLOW,
    apiKey: ROBOFLOW_API_KEY,
  },
} as const;
