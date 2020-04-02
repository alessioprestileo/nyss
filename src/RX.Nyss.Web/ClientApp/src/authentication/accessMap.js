import { Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, DataConsumer, Supervisor } from "./roles";

export const accessMap = {
  nationalSocieties: {
    showDashboard: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, DataConsumer],
    list: [Administrator, GlobalCoordinator, TechnicalAdvisor, DataConsumer],
    add: [Administrator, GlobalCoordinator],
    edit: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor],
    delete: [Administrator, GlobalCoordinator],
    archive: [Administrator, GlobalCoordinator],
    get: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor, DataConsumer],
  },
  nationalSocietyStructure: {
    edit: [Administrator, Manager, TechnicalAdvisor]
  },
  smsGateways: {
    list: [Administrator, Manager, TechnicalAdvisor],
    add: [Administrator, Manager, TechnicalAdvisor],
    edit: [Administrator, Manager, TechnicalAdvisor],
    delete: [Administrator, Manager, TechnicalAdvisor]
  },
  projects: {
    get: [Administrator, Manager, TechnicalAdvisor, DataConsumer, Supervisor],
    showOverview: [Administrator, Manager, TechnicalAdvisor, Supervisor],
    showDashboard: [Administrator, Manager, TechnicalAdvisor, DataConsumer, Supervisor],
    list: [Administrator, Manager, TechnicalAdvisor, DataConsumer],
    add: [Administrator, Manager, TechnicalAdvisor],
    edit: [Administrator, Manager, TechnicalAdvisor],
    delete: [Administrator, Manager, TechnicalAdvisor]
  },
  nationalSocietyUsers: {
    list: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor],
    add: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor],
    edit: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor],
    delete: [Administrator, GlobalCoordinator, Manager, TechnicalAdvisor],
    headManagerAccess: [GlobalCoordinator, Administrator]
  },
  globalCoordinators: {
    list: [Administrator, GlobalCoordinator],
    add: [Administrator],
    edit: [Administrator],
    delete: [Administrator]
  },
  healthRisks: {
    list: [Administrator, GlobalCoordinator],
    add: [Administrator, GlobalCoordinator],
    edit: [Administrator, GlobalCoordinator],
    delete: [Administrator, GlobalCoordinator]
  },
  dataCollectors: {
    list: [Administrator, Manager, TechnicalAdvisor, Supervisor],
    performanceList: [Administrator, Manager, TechnicalAdvisor, Supervisor],
    add: [Administrator, Manager, TechnicalAdvisor, Supervisor],
    edit: [Administrator, Manager, TechnicalAdvisor, Supervisor],
    delete: [Administrator, Manager, TechnicalAdvisor, Supervisor],
    export: [Administrator, Manager, TechnicalAdvisor]
  },
  reports: {
    list: [Administrator, Manager, TechnicalAdvisor, Supervisor],
    edit: [Administrator, Manager, TechnicalAdvisor],
    markAsError: [Administrator, Manager, TechnicalAdvisor, Supervisor],
    sendReport: [Administrator, Manager, TechnicalAdvisor, Supervisor]
  },
  nationalSocietyReports: {
    list: [Administrator, Manager, TechnicalAdvisor],
  },
  alerts: {
    list: [Administrator, Manager, TechnicalAdvisor, Supervisor],
    assess: [Administrator, Manager, TechnicalAdvisor, Supervisor],
  },
  translations: {
    list: [Administrator, GlobalCoordinator]
  }
};
