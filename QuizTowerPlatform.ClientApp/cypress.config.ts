import { defineConfig } from "cypress";
import codeCoverage from '@cypress/code-coverage/task';
import * as dotenv from "dotenv";

dotenv.config();

export default defineConfig({
  e2e: {
    setupNodeEvents(on, config) {
      codeCoverage(on, config);

      config.env = {
        ...config.env,
        ...process.env,
      };

      // It's IMPORTANT to return the config object
      // with any changed environment variables
      return config;
    },
    baseUrl: process.env['baseUrl'] || 'https://localhost:44365',
    chromeWebSecurity: false,
  }
});