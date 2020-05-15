import { call, put, takeEvery, select } from "redux-saga/effects";
import * as consts from "./nationalSocietyUsersConstants";
import * as actions from "./nationalSocietyUsersActions";
import * as appActions from "../../app/logic/appActions";
import * as http from "../../../utils/http";
import { entityTypes } from "../../nationalSocieties/logic/nationalSocietiesConstants";
import * as roles from "../../../authentication/roles";
import { stringKeys, stringKey } from "../../../strings";

export const nationalSocietyUsersSagas = () => [
  takeEvery(consts.OPEN_NATIONAL_SOCIETY_USERS_LIST.INVOKE, openNationalSocietyUsersList),
  takeEvery(consts.OPEN_NATIONAL_SOCIETY_USER_CREATION.INVOKE, openNationalSocietyUserCreation),
  takeEvery(consts.OPEN_NATIONAL_SOCIETY_USER_ADD_EXISTING.INVOKE, openNationalSocietyAddExistingUser),
  takeEvery(consts.OPEN_NATIONAL_SOCIETY_USER_EDITION.INVOKE, openNationalSocietyUserEdition),
  takeEvery(consts.CREATE_NATIONAL_SOCIETY_USER.INVOKE, createNationalSocietyUser),
  takeEvery(consts.ADD_EXISTING_NATIONAL_SOCIETY_USER.INVOKE, addExistingNationalSocietyUser),
  takeEvery(consts.EDIT_NATIONAL_SOCIETY_USER.INVOKE, editNationalSocietyUser),
  takeEvery(consts.REMOVE_NATIONAL_SOCIETY_USER.INVOKE, removeNationalSocietyUser),
  takeEvery(consts.SET_AS_HEAD_MANAGER.INVOKE, setAsHeadManagerInNationalSociety)
];

function* openNationalSocietyUsersList({ nationalSocietyId }) {
  yield put(actions.openList.request());
  try {
    yield openNationalSocietyUsersModule(nationalSocietyId);

    const isListStale = yield select(state => state.nationalSocietyUsers.listStale);
    const lastNationalSocietyId = yield select(state => state.nationalSocietyUsers.listNationalSocietyId);

    if (isListStale || lastNationalSocietyId !== nationalSocietyId) {
      yield call(getNationalSocietyUsers, nationalSocietyId);
    }

    yield put(actions.openList.success());
  } catch (error) {
    yield put(actions.openList.failure(error.message));
  }
};

function* openNationalSocietyUserCreation({ nationalSocietyId }) {
  yield put(actions.openCreation.request());
  try {
    yield openNationalSocietyUsersModule(nationalSocietyId);
    const formData = yield call(http.get, `/api/user/createFormData?nationalSocietyId=${nationalSocietyId}`);
    yield put(actions.openCreation.success(formData.value));
  } catch (error) {
    yield put(actions.openCreation.failure(error.message));
  }
};

function* openNationalSocietyAddExistingUser({ nationalSocietyId }) {
  yield put(actions.openAddExisting.request());
  try {
    yield openNationalSocietyUsersModule(nationalSocietyId);
    yield put(actions.openAddExisting.success());
  } catch (error) {
    yield put(actions.openAddExisting.failure(error.message));
  }
};

function* openNationalSocietyUserEdition({ nationalSocietyUserId, role }) {
  yield put(actions.openEdition.request());
  try {
    const nationalSocietyId = yield select(state => state.appData.route.params.nationalSocietyId);
    const formData = yield call(http.get, `/api/user/editFormData?nationalSocietyUserId=${nationalSocietyUserId}&nationalSocietyId=${nationalSocietyId}`);
    const response = yield call(http.get, getSpecificRoleUserRetrievalUrl(nationalSocietyUserId, formData.value.role, nationalSocietyId));
    yield openNationalSocietyUsersModule(nationalSocietyId);
    yield put(actions.openEdition.success(response.value, formData.value.projects, formData.value.organizations));
  } catch (error) {
    yield put(actions.openEdition.failure(error.message));
  }
};

function* createNationalSocietyUser({ nationalSocietyId, data }) {
  yield put(actions.create.request());
  try {
    const response = yield call(http.post, getSpecificRoleUserAdditionUrl(nationalSocietyId, data.role), data);
    yield put(actions.create.success(response.value));
    yield put(actions.goToList(nationalSocietyId));
    yield put(appActions.showMessage(stringKeys.nationalSocietyUser.messages.creationSuccessful));
  } catch (error) {
    yield put(actions.create.failure(error.message));
  }
};

function* addExistingNationalSocietyUser({ nationalSocietyId, data }) {
  yield put(actions.create.request());
  try {
    const response = yield call(http.post, `/api/user/addExisting?nationalSocietyId=${nationalSocietyId}`, data);
    yield put(actions.create.success(response.value));
    yield put(actions.goToList(nationalSocietyId));
    yield put(appActions.showMessage(stringKeys.nationalSocietyUser.create.success));
  } catch (error) {
    yield put(actions.create.failure(error.message));
  }
};

function* editNationalSocietyUser({ nationalSocietyId, data }) {
  yield put(actions.edit.request());
  try {
    const response = yield call(http.post, getSpecificRoleUserEditionUrl(data.id, data.role), data);
    yield put(actions.edit.success(response.value));
    yield put(actions.goToList(nationalSocietyId));
    yield put(appActions.showMessage(stringKeys.nationalSocietyUser.edit.success));
  } catch (error) {
    yield put(actions.edit.failure(error.message));
  }
};

function* removeNationalSocietyUser({ nationalSocietyUserId, role, nationalSocietyId }) {
  yield put(actions.remove.request(nationalSocietyUserId));
  try {
    yield call(http.post, getSpecificRoleUserRemovalUrl(nationalSocietyUserId, role, nationalSocietyId));
    yield put(actions.remove.success(nationalSocietyUserId));
    yield put(appActions.showMessage(stringKeys.nationalSocietyUser.remove.success));
  } catch (error) {
    yield put(actions.remove.failure(nationalSocietyUserId, error.message));
  }
};

function* getNationalSocietyUsers(nationalSocietyId) {
  yield put(actions.getList.request());
  try {
    const response = yield call(http.get, `/api/user/list?nationalSocietyId=${nationalSocietyId}`);
    yield put(actions.getList.success(nationalSocietyId, response.value));
  } catch (error) {
    yield put(actions.getList.failure(error.message));
  }
};

function* setAsHeadManagerInNationalSociety({ nationalSocietyId, nationalSocietyUserId }) {
  yield put(actions.setAsHeadManager.request(nationalSocietyUserId));
  try {
    yield call(http.post, `/api/nationalSociety/${nationalSocietyId}/setHeadManager`, { userId: nationalSocietyUserId });
    yield put(actions.setAsHeadManager.success(nationalSocietyUserId));
    yield put(appActions.showMessage(stringKeys.nationalSocietyConsents.setSuccessfully));
    yield call(getNationalSocietyUsers, nationalSocietyId);
  } catch (error) {
    yield put(actions.setAsHeadManager.failure(nationalSocietyUserId));
  }
};

function getSpecificRoleUserAdditionUrl(nationalSocietyId, role) {
  switch (role) {
    case roles.TechnicalAdvisor:
      return `/api/technicalAdvisor/create?nationalSocietyId=${nationalSocietyId}`;
    case roles.Manager:
      return `/api/manager/create?nationalSocietyId=${nationalSocietyId}`;
    case roles.DataConsumer:
      return `/api/dataConsumer/create?nationalSocietyId=${nationalSocietyId}`;
    case roles.Supervisor:
      return `/api/supervisor/create?nationalSocietyId=${nationalSocietyId}`;
    case roles.Coordinator:
      return `/api/coordinator/create?nationalSocietyId=${nationalSocietyId}`;
    default:
      throw new Error(stringKey(stringKeys.nationalSocietyUser.messages.roleNotValid));
  }
};

function getSpecificRoleUserEditionUrl(userId, role) {
  switch (role) {
    case roles.TechnicalAdvisor:
      return `/api/technicalAdvisor/${userId}/edit`;
    case roles.Manager:
      return `/api/manager/${userId}/edit`;
    case roles.DataConsumer:
      return `/api/dataConsumer/${userId}/edit`;
    case roles.Supervisor:
      return `/api/supervisor/${userId}/edit`;
    case roles.Coordinator:
      return `/api/coordinator/${userId}/edit`;
    default:
      throw new Error(stringKey(stringKeys.nationalSocietyUser.messages.roleNotValid));
  }
};

function getSpecificRoleUserRetrievalUrl(userId, role, nationalSocietyId) {
  switch (role) {
    case roles.TechnicalAdvisor:
      return `/api/technicalAdvisor/${userId}/get?nationalSocietyId=${nationalSocietyId}`;
    case roles.Manager:
      return `/api/manager/${userId}/get?nationalSocietyId=${nationalSocietyId}`;
    case roles.DataConsumer:
      return `/api/dataConsumer/${userId}/get?nationalSocietyId=${nationalSocietyId}`;
    case roles.Supervisor:
      return `/api/supervisor/${userId}/get?nationalSocietyId=${nationalSocietyId}`;
    case roles.Coordinator:
      return `/api/coordinator/${userId}/get?nationalSocietyId=${nationalSocietyId}`;
    default:
      throw new Error(stringKey(stringKeys.nationalSocietyUser.messages.roleNotValid));
  }
};

function getSpecificRoleUserRemovalUrl(userId, role, nationalSocietyId) {
  switch (role) {
    case roles.TechnicalAdvisor:
      return `/api/technicalAdvisor/${userId}/delete?nationalSocietyId=${nationalSocietyId}`;
    case roles.Manager:
      return `/api/manager/${userId}/delete`;
    case roles.DataConsumer:
      return `/api/dataConsumer/${userId}/delete?nationalSocietyId=${nationalSocietyId}`;
    case roles.Supervisor:
      return `/api/supervisor/${userId}/delete`;
    case roles.Coordinator:
      return `/api/coordinator/${userId}/delete`;
    default:
      throw new Error(stringKey(stringKeys.nationalSocietyUser.messages.roleNotValid));
  }
};

function* openNationalSocietyUsersModule(nationalSocietyId) {
  const nationalSociety = yield call(http.getCached, {
    path: `/api/nationalSociety/${nationalSocietyId}/get`,
    dependencies: [entityTypes.nationalSociety(nationalSocietyId)]
  });

  yield put(appActions.openModule.invoke(null, {
    nationalSocietyId: nationalSociety.value.id,
    nationalSocietyName: nationalSociety.value.name,
    nationalSocietyCountry: nationalSociety.value.countryName,
    nationalSocietyIsArchived: nationalSociety.value.isArchived
  }));
}
