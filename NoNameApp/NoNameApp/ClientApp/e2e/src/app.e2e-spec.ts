
import { AppPage } from './app.po';
import { browser, logging } from 'protractor';

describe('workspace-project App', () => {
  let page: AppPage;

  beforeEach(() => {
    page = new AppPage();
  });

  it('should display application title: NoNameApp', () => {
    page.navigateTo();
    expect(page.getAppTitle()).toEqual('NoNameApp');
  });
});
