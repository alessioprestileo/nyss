import styles from './StringsSwitcher.module.scss';

import React from 'react';
import PropTypes from "prop-types";
import { connect } from "react-redux";
import * as appActions from '../app/logic/appActions';
import Button from '@material-ui/core/Button';

const StringsSwitcherComponent = ({ isDevelopment, switchStrings, showStringsKeys }) => {
  if (!isDevelopment) {
    return null;
  }

  return (
    <div className={styles.stringSwitcher}>
      <Button key="undo" variant="text" color="primary" onClick={() => switchStrings()}>
        Switch keys
      </Button>
    </div>
  );
}

StringsSwitcherComponent.propTypes = {
  appReady: PropTypes.bool,
  sideMenu: PropTypes.array
};

const mapStateToProps = state => ({
  isDevelopment: state.appData.isDevelopment
});

const mapDispatchToProps = {
  switchStrings: appActions.switchStrings
};

export const StringsSwitcher = connect(mapStateToProps, mapDispatchToProps)(StringsSwitcherComponent);