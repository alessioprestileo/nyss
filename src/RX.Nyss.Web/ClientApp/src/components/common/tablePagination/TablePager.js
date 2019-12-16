import styles from "./TablePager.module.scss";
import React from 'react';
import IconButton from '@material-ui/core/IconButton';
import MenuItem from '@material-ui/core/MenuItem';
import Select from '@material-ui/core/Select';
import KeyboardArrowLeft from '@material-ui/icons/KeyboardArrowLeft';
import KeyboardArrowRight from '@material-ui/icons/KeyboardArrowRight';
import Grid from "@material-ui/core/Grid";

export const TablePager = (props) => {
  const { page, rowsPerPage, totalRows, onChangePage } = props;

  const numberOfPages = Math.ceil(totalRows / rowsPerPage);
  const pagesRange = [...Array(numberOfPages)].map((_, i) => i + 1);

  const handleBackButtonClick = event => {
    onChangePage(event, page - 1);
  };

  const handleNextButtonClick = event => {
    onChangePage(event, page + 1);
  };

  const handlePageSelect = event => {
    onChangePage(event, event.target.value);
  };

  if (numberOfPages < 2) {
    return null;
  }

  return (
    <Grid
      container
      direction="row"
      justify="flex-end"
      alignItems="center"
      className={styles.pager}
    >
      <IconButton onClick={handleBackButtonClick} disabled={page === 1} aria-label="previous page">
        <KeyboardArrowLeft />
      </IconButton>

      <Select className={`${styles.dropDown}`} onChange={handlePageSelect} value={page}>
        {pagesRange.map(page => (<MenuItem value={page} key={page}>{page}</MenuItem>))}
      </Select>

      <IconButton
        onClick={handleNextButtonClick}
        disabled={page >= Math.ceil(totalRows / rowsPerPage)}
        aria-label="next page">
        <KeyboardArrowRight />
      </IconButton>
    </Grid>
  );
};

export default TablePager;