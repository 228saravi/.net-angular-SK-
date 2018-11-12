import { Component, OnInit, ViewChild } from '@angular/core';
import { DxPopupComponent } from 'devextreme-angular';
import { ProfileCheckBeforePublish } from '../../_services/sk-domain-expert-profile-details-updator.service';

import { ProfileIsNotPublishedError } from '../../_services/sk-domain-expert-profile-details-updator.service';
import { CompanyIsNotPublishedError } from '../../_services/sk-domain-company-details-updator.service';
import { EventIsNotPublishedError } from '../../_services/sk-domain-event-details-updator.service';
import { VacancyIsNotPublishedError } from '../../_services/sk-domain-vacancy-details-updator.service';

export class CheckItem {
  public isGood: boolean;
  public text: string;

  public constructor(isGood: boolean, text: string) {
    this.isGood = isGood;
    this.text = text;
  }
}

export class CheckGroup {
  public name: string;
  public checkItems: CheckItem[];

  public constructor(name: string, checkItems: CheckItem[]) {
    this.name = name;
    this.checkItems = checkItems;
  }
}

export class CheckData {
  public h1: string;
  public h2: string;

  public checkGroups: CheckGroup[];

  public constructor(h1: string, h2: string, checkGroups: CheckGroup[]) {
    this.h1 = h1;
    this.h2 = h2;
    this.checkGroups = checkGroups;
  }

  public static fromProfileIsNotPreparedForPublishError(e: ProfileIsNotPublishedError): CheckData {
    var check = e.profileCheck;

    var checkData = new CheckData(
      "Пожалуйста, заполните недостающую информацию и опубликуйте профиль",
      "Это поможет компаниям быстрее находить Ваш профиль",
      [new CheckGroup("Для профиля", [
        new CheckItem(check.nameSet, "Имя добавлено"),
        new CheckItem(check.photoSet, "Фото добавлено"),
        new CheckItem(check.citySet, "Город добавлен"),
        new CheckItem(check.ratePerHourSet, "Ставка добавлена"),
        new CheckItem(check.specialitySet, "Специальность добавлена"),
        new CheckItem(check.specializationSet, "Специализация добавлена"),
        new CheckItem(check.experienceSet, "Стаж добавлен"),
      ])]
    );

    return checkData;
  }

  public static fromCompanyIsNotPreparedForPublishError(e: CompanyIsNotPublishedError): CheckData {
    var check = e.companyCheck;

    var checkData = new CheckData(
      "Пожалуйста, заполните недостающую информацию и опубликуйте компанию",
      "Это поможет сотрудникам быстрее находить Вашу компанию",
      [new CheckGroup("Для компании", [
        new CheckItem(check.nameSet, "Название добавлено"),
        new CheckItem(check.logoSet, "Лого добавлено"),
        new CheckItem(check.citySet, "Город добавлен"),
      ])]
    );

    return checkData;
  }

  public static fromEventIsNotPreparedForPublishError(e: EventIsNotPublishedError): CheckData {
    var check = e.eventCheck;

    var checkData = new CheckData(
      "Пожалуйста, заполните недостающую информацию и опубликуйте мероприятие",
      "Это поможет сотрудникам быстрее находить это мероприятие",
      [new CheckGroup("Для мероприятия", [
        new CheckItem(check.nameSet, "Название добавлено"),
        new CheckItem(check.logoSet, "Лого добавлено"),
        new CheckItem(check.typeSet, "Тип добавлен"),
        new CheckItem(check.formatSet, "Формат добавлен"),
        new CheckItem(check.startTimeSet, "Время начала добавлено"),
        new CheckItem(check.endTimeSet, "Время окончания добавлено"),
        new CheckItem(check.segmentSet, "Сегмент добавлен"),
        new CheckItem(check.citySet, "Город добавлен"),
        new CheckItem(check.addrerssSet, "Адрес добавлен"),
      ])]
    );

    return checkData;
  }

  public static fromVacancyIsNotPreparedForPublishError(e: VacancyIsNotPublishedError): CheckData {
    var check = e.vacancyCheck;

    var checkData = new CheckData(
      "Пожалуйста, заполните недостающую информацию и опубликуйте вакансию",
      "Это поможет компаниям быстрее находить Вашу вакансию",
      [new CheckGroup("Для профиля", [
        new CheckItem(check.specialitySet, "Специальность добавлена"),
        new CheckItem(check.specializationSet, "Специализация добавлена"),
        new CheckItem(check.amountSet, "Количество добавлено"),
        new CheckItem(check.ratePerHourSet, "Ставка добавлена"),
        new CheckItem(check.startTimeSet, "Время начала добавлено"),
        new CheckItem(check.workingHoursSet, "Количество рабочих часов добавлено"),
        new CheckItem(check.experienceSet, "Стаж добавлен"),

      ])]
    );

    return checkData;
  }
}

class Vm {
  public preventClosingMarker: boolean = true;
  public checkData: CheckData;

  public constructor(checkData: CheckData) {
    this.checkData = checkData;
  }

  public get popupHeight(): number {
    return 196 + // Высота шапки, если там везде по 2 строки
      this.checkData.checkGroups.filter(g => g.name).length * 42 + // Высота заголовков групп
      this.checkData.checkGroups.map(g => g.checkItems).reduce((arr, chunk) => [...arr, ...chunk], []).length * 30 - 12 + // Высота строк в группах
      this.checkData.checkGroups.length * 24; // Отступы после групп
  }
}

@Component({
  selector: 'sk-publish-check-list-popup',
  templateUrl: './sk-publish-check-list-popup.component.html',
  styleUrls: ['./sk-publish-check-list-popup.component.scss']
})
export class SkPublishCheckListPopupComponent implements OnInit {
  @ViewChild("popup") popup: DxPopupComponent;

  private _vm: Vm = null;

  public constructor() { }

  public get vm(): Vm {
    return this._vm;
  }

  public ngOnInit(): void {
  }

  public async show(checkData: CheckData): Promise<void> {
    this._vm = new Vm(checkData);
    await (this.popup.instance.show() as Promise<void>);
  }

  public async hide(): Promise<void> {
    this._vm.preventClosingMarker = false;
    await (this.popup.instance.hide() as Promise<void>);
  }

  public onHidding($event): void {
    if (this._vm.preventClosingMarker) {
      $event.cancel = true;
    }
  }

  public onHidden(): void {
    this._vm = null;
  }
}

