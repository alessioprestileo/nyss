import { push } from "connected-react-router";
import {
  OPEN_DATA_COLLECTORS_LIST, GET_DATA_COLLECTORS,
  OPEN_DATA_COLLECTOR_CREATION, CREATE_DATA_COLLECTOR,
  OPEN_DATA_COLLECTOR_EDITION, EDIT_DATA_COLLECTOR,
  REMOVE_DATA_COLLECTOR, OPEN_DATA_COLLECTORS_MAP_OVERVIEW,
  GET_DATA_COLLECTORS_MAP_OVERVIEW,
  GET_DATA_COLLECTORS_MAP_DETAILS,
  SET_DATA_COLLECTORS_TRAINING_STATE,
  OPEN_DATA_COLLECTORS_PERFORMANCE_LIST,
  GET_DATA_COLLECTORS_PERFORMANCE,
  EXPORT_TO_EXCEL,
  EXPORT_DATA_COLLECTORS_TO_CSV,
  EXPORT_DATA_COLLECTORS_TO_EXCEL
} from "./dataCollectorsConstants";

export const goToList = (projectId) => push(`/projects/${projectId}/datacollectors/list`);
export const goToOverview = (projectId) => push(`/projects/${projectId}/datacollectors/mapoverview`);
export const goToCreation = (projectId) => push(`/projects/${projectId}/datacollectors/add`);
export const goToEdition = (projectId, dataCollectorId) => push(`/projects/${projectId}/datacollectors/${dataCollectorId}/edit`);

export const openList = {
  invoke: (projectId) => ({ type: OPEN_DATA_COLLECTORS_LIST.INVOKE, projectId }),
  request: () => ({ type: OPEN_DATA_COLLECTORS_LIST.REQUEST }),
  success: (projectId, filtersData) => ({ type: OPEN_DATA_COLLECTORS_LIST.SUCCESS, projectId, filtersData }),
  failure: (message) => ({ type: OPEN_DATA_COLLECTORS_LIST.FAILURE, message })
};

export const getList = {
  invoke: (projectId, filters) => ({ type: GET_DATA_COLLECTORS.INVOKE, projectId, filters }),
  request: () => ({ type: GET_DATA_COLLECTORS.REQUEST }),
  success: (list, filters) => ({ type: GET_DATA_COLLECTORS.SUCCESS, list, filters }),
  failure: (message) => ({ type: GET_DATA_COLLECTORS.FAILURE, message })
};

export const openMapOverview = {
  invoke: (projectId, from, to) => ({ type: OPEN_DATA_COLLECTORS_MAP_OVERVIEW.INVOKE, projectId, from, to }),
  request: () => ({ type: OPEN_DATA_COLLECTORS_MAP_OVERVIEW.REQUEST }),
  success: () => ({ type: OPEN_DATA_COLLECTORS_MAP_OVERVIEW.SUCCESS }),
  failure: (message) => ({ type: OPEN_DATA_COLLECTORS_MAP_OVERVIEW.FAILURE, message })
};

export const getMapOverview = {
  invoke: (projectId, filters) => ({ type: GET_DATA_COLLECTORS_MAP_OVERVIEW.INVOKE, projectId, filters }),
  request: () => ({ type: GET_DATA_COLLECTORS_MAP_OVERVIEW.REQUEST }),
  success: (filters, dataCollectorLocations, centerLocation) => ({ type: GET_DATA_COLLECTORS_MAP_OVERVIEW.SUCCESS, filters, dataCollectorLocations, centerLocation }),
  failure: (message) => ({ type: GET_DATA_COLLECTORS_MAP_OVERVIEW.FAILURE, message })
};

export const openCreation = {
  invoke: (projectId) => ({ type: OPEN_DATA_COLLECTOR_CREATION.INVOKE, projectId }),
  request: () => ({ type: OPEN_DATA_COLLECTOR_CREATION.REQUEST }),
  success: (regions, supervisors, defaultLocation, defaultSupervisorId) => ({ type: OPEN_DATA_COLLECTOR_CREATION.SUCCESS, regions, supervisors, defaultLocation, defaultSupervisorId }),
  failure: (message) => ({ type: OPEN_DATA_COLLECTOR_CREATION.FAILURE, message })
};

export const create = {
  invoke: (projectId, data) => ({ type: CREATE_DATA_COLLECTOR.INVOKE, projectId, data }),
  request: () => ({ type: CREATE_DATA_COLLECTOR.REQUEST }),
  success: () => ({ type: CREATE_DATA_COLLECTOR.SUCCESS }),
  failure: (message) => ({ type: CREATE_DATA_COLLECTOR.FAILURE, message, suppressPopup: true })
};

export const openEdition = {
  invoke: (dataCollectorId) => ({ type: OPEN_DATA_COLLECTOR_EDITION.INVOKE, dataCollectorId }),
  request: () => ({ type: OPEN_DATA_COLLECTOR_EDITION.REQUEST }),
  success: (data) => ({ type: OPEN_DATA_COLLECTOR_EDITION.SUCCESS, data }),
  failure: (message) => ({ type: OPEN_DATA_COLLECTOR_EDITION.FAILURE, message })
};

export const edit = {
  invoke: (projectId, data) => ({ type: EDIT_DATA_COLLECTOR.INVOKE, projectId, data }),
  request: () => ({ type: EDIT_DATA_COLLECTOR.REQUEST }),
  success: () => ({ type: EDIT_DATA_COLLECTOR.SUCCESS }),
  failure: (message) => ({ type: EDIT_DATA_COLLECTOR.FAILURE, message, suppressPopup: true })
};

export const remove = {
  invoke: (dataCollectorId) => ({ type: REMOVE_DATA_COLLECTOR.INVOKE, dataCollectorId }),
  request: (id) => ({ type: REMOVE_DATA_COLLECTOR.REQUEST, id }),
  success: (id) => ({ type: REMOVE_DATA_COLLECTOR.SUCCESS, id }),
  failure: (id, message) => ({ type: REMOVE_DATA_COLLECTOR.FAILURE, id, message })
};

export const getMapDetails = {
  invoke: (projectId, lat, lng) => ({ type: GET_DATA_COLLECTORS_MAP_DETAILS.INVOKE, projectId, lat, lng }),
  request: () => ({ type: GET_DATA_COLLECTORS_MAP_DETAILS.REQUEST }),
  success: (details) => ({ type: GET_DATA_COLLECTORS_MAP_DETAILS.SUCCESS, details }),
  failure: (message) => ({ type: GET_DATA_COLLECTORS_MAP_DETAILS.FAILURE, message })
};

export const setTrainingState = {
  invoke: (dataCollectorId, inTraining) => ({ type: SET_DATA_COLLECTORS_TRAINING_STATE.INVOKE, dataCollectorId, inTraining }),
  request: (dataCollectorId) => ({ type: SET_DATA_COLLECTORS_TRAINING_STATE.REQUEST, dataCollectorId }),
  success: (dataCollectorId, inTraining) => ({ type: SET_DATA_COLLECTORS_TRAINING_STATE.SUCCESS, dataCollectorId, inTraining }),
  failure: (dataCollectorId) => ({ type: SET_DATA_COLLECTORS_TRAINING_STATE.FAILURE, dataCollectorId })
};

export const openDataCollectorsPerformanceList = {
  invoke: (projectId) => ({ type: OPEN_DATA_COLLECTORS_PERFORMANCE_LIST.INVOKE, projectId }),
  request: () => ({ type: OPEN_DATA_COLLECTORS_PERFORMANCE_LIST.REQUEST }),
  success: (projectId) => ({ type: OPEN_DATA_COLLECTORS_PERFORMANCE_LIST.SUCCESS, projectId }),
  failure: (message) => ({ type: OPEN_DATA_COLLECTORS_PERFORMANCE_LIST.FAILURE, message })
}

export const getDataCollectorsPerformanceList = {
  invoke: (projectId) => ({ type: GET_DATA_COLLECTORS_PERFORMANCE.INVOKE, projectId }),
  request: () => ({ type: GET_DATA_COLLECTORS_PERFORMANCE.REQUEST }),
  success: (list) => ({ type: GET_DATA_COLLECTORS_PERFORMANCE.SUCCESS, list }),
  failure: (message) => ({ type: GET_DATA_COLLECTORS_PERFORMANCE.FAILURE, message })
}

export const exportToExcel = {
  invoke: (projectId) => ({ type: EXPORT_DATA_COLLECTORS_TO_EXCEL.INVOKE, projectId }),
  request: () => ({ type: EXPORT_DATA_COLLECTORS_TO_EXCEL.REQUEST }),
  success: () => ({ type: EXPORT_DATA_COLLECTORS_TO_EXCEL.SUCCESS }),
  failure: (message) => ({ type: EXPORT_DATA_COLLECTORS_TO_EXCEL.FAILURE, message })
};

export const exportToCsv = {
  invoke: (projectId) => ({ type: EXPORT_DATA_COLLECTORS_TO_CSV.INVOKE, projectId }),
  request: () => ({ type: EXPORT_DATA_COLLECTORS_TO_CSV.REQUEST }),
  success: () => ({ type: EXPORT_DATA_COLLECTORS_TO_CSV.SUCCESS }),
  failure: (message) => ({ type: EXPORT_DATA_COLLECTORS_TO_CSV.FAILURE, message })
};
