import styles from '../common/table/Table.module.scss';
import React from 'react';
import PropTypes from "prop-types";
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import ClearIcon from '@material-ui/icons/Clear';
import EditIcon from '@material-ui/icons/Edit';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import { TableRowAction } from '../common/tableRowAction/TableRowAction';
import { Loading } from '../common/loading/Loading';
import { strings, stringKeys } from '../../strings';
import { TableRowMenu } from '../common/tableRowAction/TableRowMenu';
import { TableContainer } from '../common/table/TableContainer';
import { TableRowActions } from '../common/tableRowAction/TableRowActions';
import { accessMap } from '../../authentication/accessMap';
import { trainingStatusInTraining, trainingStatusTrained } from './logic/dataCollectorsConstants';

export const DataCollectorsTable = ({ isListFetching, isRemoving, goToEdition, remove, list, projectId, setTrainingState, isSettingTrainingState }) => {
  if (isListFetching) {
    return <Loading />;
  }

  const getRowMenu = (row) => [
    {
      title: strings(stringKeys.dataCollector.list.takeOutOfTraining),
      action: () => setTrainingState(row.id, false),
      condition: row.isInTrainingMode,
      roles: accessMap.dataCollectors.list
    },
    {
      title: strings(stringKeys.dataCollector.list.setToInTraining),
      action: () => setTrainingState(row.id, true),
      condition: !row.isInTrainingMode,
      roles: accessMap.dataCollectors.list
    }
  ];

  return (
    <TableContainer sticky>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>{strings(stringKeys.dataCollector.list.dataCollectorType)}</TableCell>
            <TableCell>{strings(stringKeys.dataCollector.list.name)}</TableCell>
            <TableCell>{strings(stringKeys.dataCollector.list.displayName)}</TableCell>
            <TableCell>{strings(stringKeys.dataCollector.list.phoneNumber)}</TableCell>
            <TableCell>{strings(stringKeys.dataCollector.list.sex)}</TableCell>
            <TableCell>{strings(stringKeys.dataCollector.list.location)}</TableCell>
            <TableCell>{strings(stringKeys.dataCollector.list.trainingStatus)}</TableCell>
            <TableCell>{strings(stringKeys.dataCollector.list.supervisor)}</TableCell>
            <TableCell />
          </TableRow>
        </TableHead>
        <TableBody>
          {list.map(row => (
            <TableRow key={row.id} hover onClick={() => goToEdition(projectId, row.id)} className={styles.clickableRow}>
              <TableCell>{strings(stringKeys.dataCollector.constants.dataCollectorType[row.dataCollectorType])}</TableCell>
              <TableCell>{row.name}</TableCell>
              <TableCell>{row.displayName}</TableCell>
              <TableCell>{row.phoneNumber}</TableCell>
              <TableCell>{row.sex}</TableCell>
              <TableCell>{row.region}, {row.district}, {row.village}</TableCell>
              <TableCell>{row.isInTrainingMode ? strings(stringKeys.dataCollector.constants.trainingStatus[trainingStatusInTraining]) : strings(stringKeys.dataCollector.constants.trainingStatus[trainingStatusTrained])}</TableCell>
              <TableCell>{row.supervisor.name}</TableCell>
              <TableCell>
                <TableRowActions>
                  <TableRowMenu id={row.id} items={getRowMenu(row)} icon={<MoreVertIcon />} isFetching={isSettingTrainingState[row.id]} />
                  <TableRowAction onClick={() => goToEdition(projectId, row.id)} icon={<EditIcon />} title={"Edit"} />
                  <TableRowAction onClick={() => remove(row.id)} confirmationText={strings(stringKeys.dataCollector.list.removalConfirmation)} icon={<ClearIcon />} title={"Delete"} isFetching={isRemoving[row.id]} />
                </TableRowActions>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}

DataCollectorsTable.propTypes = {
  isFetching: PropTypes.bool,
  list: PropTypes.array
};

export default DataCollectorsTable;
