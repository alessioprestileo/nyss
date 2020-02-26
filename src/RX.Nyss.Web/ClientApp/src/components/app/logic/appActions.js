import * as actions from "./appConstans";
import { push } from "connected-react-router";

export const showMessage = (messageKey, time) => ({ type: actions.SHOW_MESSAGE.INVOKE, messageKey, time });
export const closeMessage = () => ({ type: actions.CLOSE_MESSAGE.INVOKE });
export const entityUpdated = (entity) => ({ type: actions.ENTITY_UPDATED, entities: [entity] });
export const switchStrings = (status) => ({ type: actions.SWITCH_STRINGS, status });
export const setAppReady = (status) => ({ type: actions.SET_APP_READY, status });
export const toggleSideMenu = (value) => ({ type: actions.TOGGLE_SIDE_MENU, value });
export const goToHeadManagerConsents = () => push(`/headManagerConsents`);
export const goToTranslations = () => push(`/translations`);
export const pageFocused = () => ({ type: actions.PAGE_FOCUSED });

export const initApplication = {
  invoke: () =>
    ({ type: actions.INIT_APPLICATION.INVOKE }),

  request: () =>
    ({ type: actions.INIT_APPLICATION.REQUEST }),

  success: () =>
    ({ type: actions.INIT_APPLICATION.SUCCESS }),

  failure: (message) =>
    ({ type: actions.INIT_APPLICATION.FAILURE, message })
};

export const getUser = {
  invoke: () =>
    ({ type: actions.GET_USER.INVOKE }),

  request: () =>
    ({ type: actions.GET_USER.REQUEST }),

  success: (isAuthenticated, user) =>
    ({ type: actions.GET_USER.SUCCESS, isAuthenticated, user }),

  failure: (message) => ({ type: actions.GET_USER.FAILURE, message })
};

export const getAppData = {
  invoke: () =>
    ({ type: actions.GET_APP_DATA.INVOKE }),

  request: () =>
    ({ type: actions.GET_APP_DATA.REQUEST }),

  success: (contentLanguages, countries, isDevelopment, authCookieExpiration) =>
    ({ type: actions.GET_APP_DATA.SUCCESS, contentLanguages, countries, isDevelopment, authCookieExpiration }),

  failure: (message) =>
    ({ type: actions.GET_APP_DATA.FAILURE, message })
};

export const getStrings = {
  invoke: () =>
    ({ type: actions.GET_STRINGS.INVOKE }),

  request: () =>
    ({ type: actions.GET_STRINGS.REQUEST }),

  success: () =>
    ({ type: actions.GET_STRINGS.SUCCESS }),

  failure: (message) =>
    ({ type: actions.GET_STRINGS.FAILURE, message })
};

export const openModule = {
  invoke: (path, params) =>
    ({ type: actions.OPEN_MODULE.INVOKE, path, params }),

  success: (path, parameters, breadcrumb, topMenu, sideMenu, tabMenu, title) =>
    ({ type: actions.OPEN_MODULE.SUCCESS, path, parameters, breadcrumb, topMenu, sideMenu, tabMenu, title }),

  failure: (message) =>
    ({ type: actions.OPEN_MODULE.FAILURE, message })
};
