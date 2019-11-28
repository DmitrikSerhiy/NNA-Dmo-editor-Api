import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

import { AppTranslationService } from './app-translation.service';
import { LocalStoreManager } from './local-store-manager.service';
import { DBkeys } from './db-keys';
import { Utilities } from './utilities';
import { environment } from '../../environments/environment';

interface UserConfiguration {
  language: string;
  homeUrl: string;
  showDashboardNotifications: boolean;
}

@Injectable()
export class ConfigurationService {

  constructor(
    private localStorage: LocalStoreManager,
    private translationService: AppTranslationService) {

    this.loadLocalChanges();
  }

  set language(value: string) {
    this._language = value;
    this.saveToLocalStore(value, DBkeys.LANGUAGE);
    this.translationService.changeLanguage(value);
  }
  get language() {
    return this._language || ConfigurationService.defaultLanguage;
  }

  set homeUrl(value: string) {
    this._homeUrl = value;
    this.saveToLocalStore(value, DBkeys.HOME_URL);
  }
  get homeUrl() {
    return this._homeUrl || ConfigurationService.defaultHomeUrl;
  }

  set showDashboardNotifications(value: boolean) {
    this._showDashboardNotifications = value;
    this.saveToLocalStore(value, DBkeys.SHOW_DASHBOARD_NOTIFICATIONS);
  }
  get showDashboardNotifications() {
    return this._showDashboardNotifications != null ? this._showDashboardNotifications : ConfigurationService.defaultShowDashboardNotifications;
  }
  public static readonly appVersion: string = '3.0.0';

  // ***Specify default configurations here***
  public static readonly defaultLanguage: string = 'en';
  public static readonly defaultHomeUrl: string = '/';
  public static readonly defaultThemeId: number = 6;
  public static readonly defaultShowDashboardNotifications: boolean = true;

  public baseUrl = environment.baseUrl || Utilities.baseUrl();
  public tokenUrl = environment.tokenUrl || environment.baseUrl || Utilities.baseUrl();
  public loginUrl = environment.loginUrl;
  public fallbackBaseUrl = 'http://quickapp.ebenmonney.com';
  // ***End of defaults***

  private _language: string = null;
  private _homeUrl: string = null;
  private _showDashboardNotifications: boolean = null;

  private onConfigurationImported: Subject<boolean> = new Subject<boolean>();
  configurationImported$ = this.onConfigurationImported.asObservable();

  private loadLocalChanges() {

    if (this.localStorage.exists(DBkeys.LANGUAGE)) {
      this._language = this.localStorage.getDataObject<string>(DBkeys.LANGUAGE);
      this.translationService.changeLanguage(this._language);
    } else {
      this.resetLanguage();
    }

    if (this.localStorage.exists(DBkeys.HOME_URL)) {
      this._homeUrl = this.localStorage.getDataObject<string>(DBkeys.HOME_URL);
    }

    if (this.localStorage.exists(DBkeys.SHOW_DASHBOARD_NOTIFICATIONS)) {
      this._showDashboardNotifications = this.localStorage.getDataObject<boolean>(DBkeys.SHOW_DASHBOARD_NOTIFICATIONS);
    }
  }

  private saveToLocalStore(data: any, key: string) {
    setTimeout(() => this.localStorage.savePermanentData(data, key));
  }


  public import(jsonValue: string) {

    this.clearLocalChanges();

    if (jsonValue) {
      const importValue: UserConfiguration = Utilities.JsonTryParse(jsonValue);

      if (importValue.language != null) {
        this.language = importValue.language;
      }

      if (importValue.homeUrl != null) {
        this.homeUrl = importValue.homeUrl;
      }

      if (importValue.showDashboardNotifications != null) {
        this.showDashboardNotifications = importValue.showDashboardNotifications;
      }
    }

    this.onConfigurationImported.next();
  }


  public export(changesOnly = true): string {

    const exportValue: UserConfiguration = {
      language: changesOnly ? this._language : this.language,
      homeUrl: changesOnly ? this._homeUrl : this.homeUrl,
      showDashboardNotifications: changesOnly ? this._showDashboardNotifications : this.showDashboardNotifications
    };

    return JSON.stringify(exportValue);
  }


  public clearLocalChanges() {
    this._language = null;
    this._homeUrl = null;
    this._showDashboardNotifications = null;
    this.localStorage.deleteData(DBkeys.LANGUAGE);
    this.localStorage.deleteData(DBkeys.HOME_URL);
    this.localStorage.deleteData(DBkeys.SHOW_DASHBOARD_NOTIFICATIONS);
    this.resetLanguage();
  }


  private resetLanguage() {
    const language = this.translationService.useBrowserLanguage();

    if (language) {
      this._language = language;
    } else {
      this._language = this.translationService.useDefaultLangage();
    }
  }
}
