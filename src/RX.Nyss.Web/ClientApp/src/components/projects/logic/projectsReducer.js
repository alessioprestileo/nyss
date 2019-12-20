import * as actions from "./projectsConstants";
import { initialState } from "../../../initialState";
import { setProperty } from "../../../utils/immutable";
import { LOCATION_CHANGE } from "connected-react-router";

export function projectsReducer(state = initialState.projects, action) {
  switch (action.type) {
    case LOCATION_CHANGE: // cleanup
      return { ...state, formData: null, formError: null }

    case actions.GET_PROJECTS.REQUEST:
      return { ...state, listFetching: true, listData: [] };

    case actions.GET_PROJECTS.SUCCESS:
      return { ...state, listFetching: false, listData: action.list, listStale: true };

    case actions.GET_PROJECTS.FAILURE:
      return { ...state, listFetching: false, listData: [] };

    case actions.OPEN_PROJECT_CREATION.INVOKE:
      return { ...state, formFetching: true, formHealthRisks: [] , formTimeZones: []};

    case actions.OPEN_PROJECT_CREATION.REQUEST:
      return { ...state, formFetching: true, formHealthRisks: [], formTimeZones: [] };

    case actions.OPEN_PROJECT_CREATION.SUCCESS:
      return { ...state, formFetching: false, formHealthRisks: action.healthRisks, formTimeZones: action.timeZones};

    case actions.OPEN_PROJECT_CREATION.FAILURE:
      return { ...state, formFetching: false };

    case actions.OPEN_PROJECT_EDITION.INVOKE:
      return { ...state, formFetching: true, formData: null, formHealthRisks: [], formTimeZones: [] };

    case actions.OPEN_PROJECT_EDITION.REQUEST:
      return { ...state, formFetching: true, formData: null, formHealthRisks: [], formTimeZones: [] };

    case actions.OPEN_PROJECT_EDITION.SUCCESS:
      return { ...state, formFetching: false, formData: action.data, formHealthRisks: action.healthRisks, formTimeZones: action.timeZones };

    case actions.OPEN_PROJECT_EDITION.FAILURE:
      return { ...state, formFetching: false };

    case actions.OPEN_PROJECT_OVERVIEW.INVOKE:
      return { ...state, formFetching: true, formData: null, formHealthRisks: [], formTimeZones: [] };

    case actions.CREATE_PROJECT.REQUEST:
      return { ...state, formSaving: true };

    case actions.CREATE_PROJECT.SUCCESS:
      return { ...state, formSaving: false, listStale: true };

    case actions.CREATE_PROJECT.FAILURE:
      return { ...state, formSaving: false, formError: action.message };

    case actions.EDIT_PROJECT.REQUEST:
      return { ...state, formSaving: true };

    case actions.EDIT_PROJECT.SUCCESS:
      return { ...state, formSaving: false, listStale: true };

    case actions.EDIT_PROJECT.FAILURE:
      return { ...state, formSaving: false, formError: action.message };

    case actions.REMOVE_PROJECT.REQUEST:
      return { ...state, listRemoving: setProperty(state.listRemoving, action.id, true) };

    case actions.REMOVE_PROJECT.SUCCESS:
      return { ...state, listRemoving: setProperty(state.listRemoving, action.id, undefined), listStale: true };

    case actions.REMOVE_PROJECT.FAILURE:
      return { ...state, listRemoving: setProperty(state.listRemoving, action.id, undefined) };

    default:
      return state;
  }
};
