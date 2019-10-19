import React, { PureComponent } from 'react';
import PropTypes from "prop-types";
import Button from '@material-ui/core/Button';
import { useLayout } from '../../utils/layout';
import { connect } from "react-redux";
import { AnonymousLayout } from '../layout/AnonymousLayout';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import Link from '@material-ui/core/Link';
import styles from './LoginPage.module.scss';
import strings from '../../strings';
import { createForm, validators } from '../../utils/forms';
import TextInputField from '../forms/TextInputField';
import PasswordInputField from '../forms/PasswordInputField';
import * as authActions from '../../authentication/authActions';
import { getRedirectUrl } from '../../authentication/auth';

class LoginPageComponent extends PureComponent {
  constructor(props) {
    super(props);

    const validation = {
      userName: [validators.required, validators.email],
      password: [validators.required, validators.minLength(8)]
    };

    const fields = {
      userName: "",
      password: ""
    };

    this.form = createForm(fields, validation);
  };

  handleSubmit = (e) => {
    e.preventDefault();
    this.onSubmit();
  };

  onSubmit = () => {
    if (!this.form.isValid()) {
      return;
    };

    const values = this.form.getValues();

    this.props.login({
      userName: values.userName,
      password: values.password,
      redirectUrl: getRedirectUrl()
    });
  };

  render() {
    return (
      <div className={styles.loginContent}>
        <Paper className={styles.loginPaper}>
          <div className={styles.loginPaperContent}>
            <Typography variant="h1" className={styles.paperHeader}>Welcome to Nyss</Typography>
            <Typography variant="h2">Log in</Typography>

            <form onSubmit={this.handleSubmit}>
              <TextInputField
                label="User name"
                name="userName"
                field={this.form.fields.userName}
                autoFocus
              />

              <PasswordInputField
                label="Password"
                name="password"
                field={this.form.fields.password}
              />

              <div className={styles.forgotPasswordLink}>
                <Link color="secondary" href={"#"}>
                  {strings["login.forgotPassword"]}
                </Link>
              </div>

              <div className={styles.actions}>
                <Button type="submit" variant="outlined" color="primary" style={{ padding: "10px 55px" }}>
                  {strings["login.signIn"]}
                </Button>
              </div>
            </form>
          </div>
        </Paper>
      </div>
    );
  }
}


const mapStateToProps = state => ({

});

const mapDispatchToProps = {
  login: authActions.login.invoke
};

LoginPageComponent.propTypes = {
  login: PropTypes.func
};

export const LoginPage = useLayout(
  AnonymousLayout,
  connect(mapStateToProps, mapDispatchToProps)(LoginPageComponent)
);
