import React, { useState, Fragment } from 'react';
import { connect } from "react-redux";
import { useLayout } from '../../utils/layout';
import { validators, createForm } from '../../utils/forms';
import * as dataCollectorsActions from './logic/dataCollectorsActions';
import Layout from '../layout/Layout';
import Form from '../forms/form/Form';
import FormActions from '../forms/formActions/FormActions';
import SubmitButton from '../forms/submitButton/SubmitButton';
import Typography from '@material-ui/core/Typography';
import TextInputField from '../forms/TextInputField';
import SelectInput from '../forms/SelectField';
import MenuItem from "@material-ui/core/MenuItem";
import SnackbarContent from '@material-ui/core/SnackbarContent';
import Button from "@material-ui/core/Button";
import { useMount } from '../../utils/lifecycle';
import { strings, stringKeys } from '../../strings';
import Grid from '@material-ui/core/Grid';
import { sexValues } from './logic/dataCollectorsConstants';

const DataCollectorsCreatePageComponent = (props) => {
  const [form] = useState(() => {
    const fields = {
      name: "",
      displayName: "",
      sex: "",
      supervisorId: "",
      dataCollectorType: "",
      birthYearGroup: "",
      additionalPhoneNumber: "",
      latitude: "",
      longitude: "",
      phoneNumber: "",
      village: "",
      district: "",
      region: "",
      zone: ""
    };

    const validation = {
      name: [validators.required, validators.minLength(1), validators.maxLength(100)],
      displayName: [validators.required, validators.minLength(1), validators.maxLength(100)],
      sex: [validators.required],
      supervisorId: [validators.required],
      dataCollectorType: [validators.required],
      birthYearGroup: [validators.required, validators.moduloTen],
      additionalPhoneNumber: [validators.phoneNumber],
      latitude: [validators.required],
      longitude: [validators.required],
      phoneNumber: [validators.required, validators.phoneNumber],
      village: [validators.required],
      district: [validators.required],
      region: [validators.required],
      zone: []
    };

    return createForm(fields, validation);
  });

  useMount(() => {
    props.openCreation(props.projectId);
    props.getCountryLocation(props.countryName);
  })

  const handleSubmit = (e) => {
    e.preventDefault();

    if (!form.isValid()) {
      return;
    };

    props.create(props.projectId, form.getValues());
  };

  return (
    <Fragment>
      <Typography variant="h2">{strings(stringKeys.dataCollector.form.creationTitle)}</Typography>

      {props.error &&
        <SnackbarContent
          message={props.error}
        />
      }

      <Form onSubmit={handleSubmit}>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <TextInputField
              label={strings(stringKeys.dataCollector.form.name)}
              name="name"
              field={form.fields.name}
            />
          </Grid>

          <Grid item xs={12}>
            <TextInputField
              label={strings(stringKeys.dataCollector.form.displayName)}
              name="displayName"
              field={form.fields.displayName}
            />
          </Grid>

          <Grid item xs={12}>
            <SelectInput
              label={strings(stringKeys.dataCollector.form.sex)}
              field={form.fields.sex}
              name="sex"
            >
              {sexValues.map(type => (
                <MenuItem key={`sex${type}`} value={type}>
                  {strings(stringKeys.dataCollector.constants.sex[type.toLowerCase()])}
                </MenuItem>
              ))}
            </SelectInput>
          </Grid>

          <Grid item xs={12}>
            <TextInputField
              label={strings(stringKeys.dataCollector.form.birthYearGroup)}
              name="birthYearGroup"
              field={form.fields.birthYearGroup}
            />
          </Grid>

          <Grid item xs={12}>
            <TextInputField
              label={strings(stringKeys.dataCollector.form.phoneNumber)}
              name="phoneNumber"
              field={form.fields.phoneNumber}
            />
          </Grid>

          <Grid item xs={12}>
            <TextInputField
              label={strings(stringKeys.dataCollector.form.additionalPhoneNumber)}
              name="additionalPhoneNumber"
              field={form.fields.additionalPhoneNumber}
            />
          </Grid>

          <Grid item xs={12}>
            
          </Grid>

          <Grid item xs={6}>
            <TextInputField
              label={strings(stringKeys.dataCollector.form.latitude)}
              name="latitude"
              field={form.fields.latitude}
            />
          </Grid>

          <Grid item xs={6}>
            <TextInputField
              label={strings(stringKeys.dataCollector.form.longitude)}
              name="longitude"
              field={form.fields.longitude}
            />
          </Grid>

          <Grid item xs={12}>
            <SelectInput
              label={strings(stringKeys.dataCollector.form.region)}
              field={form.fields.region}
              name="region"
            >
            </SelectInput>
          </Grid>

          <Grid item xs={12}>
            <SelectInput
              label={strings(stringKeys.dataCollector.form.district)}
              field={form.fields.district}
              name="district"
            >
            </SelectInput>
          </Grid>

          <Grid item xs={12}>
            <SelectInput
              label={strings(stringKeys.dataCollector.form.village)}
              field={form.fields.village}
              name="village"
            >
            </SelectInput>
          </Grid>

          <Grid item xs={12}>
            <SelectInput
              label={strings(stringKeys.dataCollector.form.zone)}
              field={form.fields.zone}
              name="zone"
            >
            </SelectInput>
          </Grid>
        </Grid>

        <FormActions>
          <Button onClick={() => props.goToList(props.projectId)}>{strings(stringKeys.form.cancel)}</Button>
          <SubmitButton isFetching={props.isSaving}>{strings(stringKeys.dataCollector.form.create)}</SubmitButton>
        </FormActions>
      </Form>
    </Fragment>
  );
}

DataCollectorsCreatePageComponent.propTypes = {
};

const mapStateToProps = (state, ownProps) => ({
  projectId: ownProps.match.params.projectId,
  countryName: state.appData.siteMap.parameters.nationalSocietyCountry,
  isSaving: state.dataCollectors.formSaving,
  isGettingCountryLocation: state.dataCollectors.gettingLocation,
  country: state.dataCollectors.countryData,
  error: state.dataCollectors.formError
});

const mapDispatchToProps = {
  openCreation: dataCollectorsActions.openCreation.invoke,
  getCountryLocation: dataCollectorsActions.getCountryLocation.invoke,
  create: dataCollectorsActions.create.invoke,
  goToList: dataCollectorsActions.goToList
};

export const DataCollectorsCreatePage = useLayout(
  Layout,
  connect(mapStateToProps, mapDispatchToProps)(DataCollectorsCreatePageComponent)
);
