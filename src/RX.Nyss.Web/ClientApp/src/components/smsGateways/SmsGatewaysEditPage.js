import React, { useEffect, useState, Fragment } from 'react';
import { connect } from "react-redux";
import { useLayout } from '../../utils/layout';
import { validators, createForm } from '../../utils/forms';
import * as smsGatewaysActions from './logic/smsGatewaysActions';
import Layout from '../layout/Layout';
import Form from '../forms/form/Form';
import FormActions from '../forms/formActions/FormActions';
import SubmitButton from '../forms/submitButton/SubmitButton';
import TextInputField from '../forms/TextInputField';
import SelectInput from '../forms/SelectField';
import MenuItem from "@material-ui/core/MenuItem";
import Button from "@material-ui/core/Button";
import { Loading } from '../common/loading/Loading';
import { smsGatewayTypes, smsEagle } from "./logic/smsGatewayTypes";
import { useMount } from '../../utils/lifecycle';
import { strings, stringKeys } from '../../strings';
import Grid from '@material-ui/core/Grid';
import { ValidationMessage } from '../forms/ValidationMessage';
import CheckboxField from '../forms/CheckboxField';
import * as roles from '../../authentication/roles'

const SmsGatewaysEditPageComponent = (props) => {
  const [form, setForm] = useState(null);

  useMount(() => {
    props.openEdition(props.nationalSocietyId, props.smsGatewayId);
  });


  const canEditIotHub = props.user && props.user.roles.some(userRole => userRole === roles.Administrator);

  useEffect(() => {
    if (!props.data) {
      return;
    }

    const fields = {
      id: props.data.id,
      name: props.data.name,
      apiKey: props.data.apiKey,
      gatewayType: props.data.gatewayType.toString(),
      emailAddress: props.data.emailAddress,
      useIotHub: props.data.useIotHub,
      iotHubDeviceName: props.data.iotHubDeviceName
    };

    const validation = {
      name: [validators.required, validators.minLength(1), validators.maxLength(100)],
      apiKey: [validators.required, validators.minLength(1), validators.maxLength(100)],
      gatewayType: [validators.required],
      emailAddress: [validators.emailWhen(_ => _.gatewayType.toString() === smsEagle)]
    };

    setForm(createForm(fields, validation));
  }, [props.data, props.match]);

  const handleSubmit = (e) => {
    e.preventDefault();

    if (!form.isValid()) {
      return;
    };

    const values = form.getValues();
    props.edit(props.nationalSocietyId, {
      id: values.id,
      name: values.name,
      apiKey: values.apiKey,
      gatewayType: values.gatewayType,
      emailAddress: values.emailAddress,
      useIotHub: values.useIotHub
    });
  };

  if (props.isFetching || !form) {
    return <Loading />;
  }

  return (
    <Fragment>
      {props.error && <ValidationMessage message={props.error} />}

      <Form onSubmit={handleSubmit}>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <TextInputField
              label={strings(stringKeys.smsGateway.form.name)}
              name="name"
              field={form.fields.name}
            />
          </Grid>

          <Grid item xs={12}>
            <TextInputField
              label={strings(stringKeys.smsGateway.form.apiKey)}
              name="apiKey"
              field={form.fields.apiKey}
            />
          </Grid>

          <Grid item xs={12}>
            <TextInputField
              label={strings(stringKeys.smsGateway.form.emailAddress)}
              name="emailAddress"
              field={form.fields.emailAddress}
            />
          </Grid>

          <Grid item xs={12}>
            <SelectInput
              label={strings(stringKeys.smsGateway.form.gatewayType)}
              name="gatewayType"
              field={form.fields.gatewayType}
            >
              {smsGatewayTypes.map(type => (
                <MenuItem
                  key={`gatewayType${type}`}
                  value={type}>
                  {strings(`smsGateway.type.${type.toLowerCase()}`)}
                </MenuItem>
              ))}
            </SelectInput>
          </Grid>
          {canEditIotHub &&
            <Grid item xs={12}>
              <CheckboxField label={strings(stringKeys.smsGateway.form.useIotHub)} field={form.fields.useIotHub}></CheckboxField>
            </Grid>
          }

          {props.data && props.data.useIotHub && canEditIotHub && (
            <Fragment>
              <Grid item xs={12}>
                <TextInputField
                  label={strings(stringKeys.smsGateway.form.iotHubDeviceName)}
                  disabled
                  field={form.fields.iotHubDeviceName}
                />
              </Grid>
              <Grid item xs={12}>
                <SubmitButton regular onClick={() => props.pingIotDevice(props.smsGatewayId)} isFetching={props.pinging}>
                  {strings(stringKeys.smsGateway.form.ping)}
                </SubmitButton>
              </Grid>
            </Fragment>
          )}
        </Grid>

        <FormActions>
          <Button onClick={() => props.goToList(props.nationalSocietyId)}>{strings(stringKeys.form.cancel)}</Button>
          <SubmitButton isFetching={props.isSaving}>{strings(stringKeys.smsGateway.form.update)}</SubmitButton>
        </FormActions>
      </Form>
    </Fragment>
  );
}

SmsGatewaysEditPageComponent.propTypes = {
};

const mapStateToProps = (state, ownProps) => ({
  smsGatewayId: ownProps.match.params.smsGatewayId,
  nationalSocietyId: ownProps.match.params.nationalSocietyId,
  isFetching: state.smsGateways.formFetching,
  isSaving: state.smsGateways.formSaving,
  data: state.smsGateways.formData,
  error: state.smsGateways.formError,
  pinging: state.smsGateways.pinging,
  user: state.appData.user
});

const mapDispatchToProps = {
  openEdition: smsGatewaysActions.openEdition.invoke,
  edit: smsGatewaysActions.edit.invoke,
  goToList: smsGatewaysActions.goToList,
  pingIotDevice: smsGatewaysActions.pingIotDevice.invoke
};

export const SmsGatewaysEditPage = useLayout(
  Layout,
  connect(mapStateToProps, mapDispatchToProps)(SmsGatewaysEditPageComponent)
);
