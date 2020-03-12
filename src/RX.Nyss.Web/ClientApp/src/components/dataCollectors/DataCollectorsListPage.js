import React, { Fragment } from 'react';
import PropTypes from "prop-types";
import { connect } from "react-redux";
import * as dataCollectorsActions from './logic/dataCollectorsActions';
import { useLayout } from '../../utils/layout';
import Layout from '../layout/Layout';
import AddIcon from '@material-ui/icons/Add';
import TableActions from '../common/tableActions/TableActions';
import DataCollectorsTable from './DataCollectorsTable';
import { useMount } from '../../utils/lifecycle';
import { strings, stringKeys } from '../../strings';
import { TableActionsButton } from '../common/tableActions/TableActionsButton';
import { accessMap } from '../../authentication/accessMap';

const DataCollectorsListPageComponent = (props) => {
  useMount(() => {
    props.openDataCollectorsList(props.projectId);
  });

  return (
    <Fragment>
      {!props.isClosed &&
        <TableActions>
          <TableActionsButton onClick={() => props.goToCreation(props.projectId)} icon={<AddIcon />}>
            {strings(stringKeys.dataCollector.addNew)}
          </TableActionsButton>
          <TableActionsButton onClick={() => props.exportToExcel(props.projectId)} roles={accessMap.dataCollectors.export}>
            {strings(stringKeys.dataCollector.exportExcel)}
          </TableActionsButton>
          <TableActionsButton onClick={() => props.exportToCsv(props.projectId)} roles={accessMap.dataCollectors.export}>
            {strings(stringKeys.dataCollector.exportCsv)}
          </TableActionsButton>
        </TableActions>
      }

      <DataCollectorsTable
        list={props.list}
        goToEdition={props.goToEdition}
        goToDashboard={props.goToDashboard}
        isListFetching={props.isListFetching}
        isRemoving={props.isRemoving}
        remove={props.remove}
        projectId={props.projectId}
        setTrainingState={props.setTrainingState}
        isSettingTrainingState={props.isSettingTrainingState}
      />
    </Fragment>
  );
}

DataCollectorsListPageComponent.propTypes = {
  getDataCollectors: PropTypes.func,
  goToCreation: PropTypes.func,
  goToEdition: PropTypes.func,
  remove: PropTypes.func,
  isFetching: PropTypes.bool,
  list: PropTypes.array,
  isClosed: PropTypes.bool
};

const mapStateToProps = (state, ownProps) => ({
  projectId: ownProps.match.params.projectId,
  isClosed: state.appData.siteMap.parameters.projectIsClosed,
  list: state.dataCollectors.listData,
  isListFetching: state.dataCollectors.listFetching,
  isRemoving: state.dataCollectors.listRemoving,
  isSettingTrainingState: state.dataCollectors.settingTrainingState
});

const mapDispatchToProps = {
  openDataCollectorsList: dataCollectorsActions.openList.invoke,
  goToCreation: dataCollectorsActions.goToCreation,
  goToEdition: dataCollectorsActions.goToEdition,
  remove: dataCollectorsActions.remove.invoke,
  setTrainingState: dataCollectorsActions.setTrainingState.invoke,
  exportToExcel: dataCollectorsActions.exportToExcel.invoke,
  exportToCsv: dataCollectorsActions.exportToCsv.invoke
};

export const DataCollectorsListPage = useLayout(
  Layout,
  connect(mapStateToProps, mapDispatchToProps)(DataCollectorsListPageComponent)
);
