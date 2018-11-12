import { Component, OnInit, Input, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { ParamMap } from '@angular/router';
import { HttpParams } from '@angular/common/http';

import { SkillsMatrixDirectory, SkillsMatrix, Speciality, Skill, Specialization } from '../../_services/sk-domain-skills-matrix-directory.service';

import DataSource from "devextreme/data/data_source";

export class SpecialitiesSelectionData {
  public selectedSpecialitiesIds: string[] = [];
  public selectedSpecializationsIds: string[] = [];
  public selectedSkillsIds: string[] = [];

  public constructor(selectedSpecialitiesIds: string[] = [], selectedSpecializationsIds: string[] = [], selectedSkillsIds: string[] = []) {
    this.selectedSpecialitiesIds = selectedSpecialitiesIds;
    this.selectedSpecializationsIds = selectedSpecializationsIds;
    this.selectedSkillsIds = selectedSkillsIds;
  }

  public static fromParamMap(paramMap: ParamMap): SpecialitiesSelectionData {
    return new SpecialitiesSelectionData(paramMap.getAll("speciality"), paramMap.getAll("specialization"), paramMap.getAll("skill"));
  }

  public toHttpParams(): HttpParams {
    var params = new HttpParams();

    for (var specialityId of this.selectedSpecialitiesIds) {
      params = params.append("speciality", specialityId);
    }

    for (var specializationId of this.selectedSpecializationsIds) {
      params = params.append("specialization", specializationId);
    }

    for (var skillId of this.selectedSkillsIds) {
      params = params.append("skill", skillId);
    }

    return params;
  }
}

class  SpecializationSelectionVm {
  public selectedId: string = null;
  public awailableSpecializations: Specialization[] = [];

  public constructor(awailableSpecializations: Specialization[]) {
    this.selectedId = null;
    this.awailableSpecializations = awailableSpecializations;
  }
}

class SkillSelectionVm {
  public selectedId: string = null;
  public awailableSkills: Skill[] = [];

  public constructor(awailableSkills: Skill[]) {
    this.selectedId = null;
    this.awailableSkills = awailableSkills;
  }
}

class SpecialitySelectionVm {
  private _awailableSpecialities: Speciality[] = [];
  private _specializationSelectionVms: SpecializationSelectionVm[] = [];
  private _skillSelectionVms: SkillSelectionVm[] = [];

  private get notSelectedSpecializations(): Specialization[] {
    if (this.selectedSpeciality) {
      return this.selectedSpeciality.specializations.filter(specialization => !this._specializationSelectionVms.some(vm => vm.selectedId == specialization.id));
    } else {
      return [];
    }
  }

  private get notSelectedSkills(): Skill[] {
    if (this.selectedSpeciality) {
      return this.selectedSpeciality.skills.filter(skill => !this._skillSelectionVms.some(vm => vm.selectedId == skill.id));
    } else {
      return [];
    }
  }

  public constructor(awailableSpecialities: Speciality[]) {
    this.selectedId = null;
    this.normalize(awailableSpecialities);
  }

  public selectedId: string = null;

  public get selectedSpeciality(): Speciality {
    return this.awailableSpecialities.find(s => s.id == this.selectedId) || null;
  }

  public get awailableSpecialities(): Speciality[] {
    return this._awailableSpecialities;
  }

  public get specializationSelectionVms(): SpecializationSelectionVm[] {
    return this._specializationSelectionVms;
  }

  public get skillSelectionVms(): SkillSelectionVm[] {
    return this._skillSelectionVms;
  }

  public normalize(awailableSpecialities: Speciality[]): void {
    this._awailableSpecialities = awailableSpecialities;

    this._skillSelectionVms = this._skillSelectionVms.filter(s => s.selectedId != null).filter(s => this.selectedSpeciality.skills.some(sk => sk.id == s.selectedId));
    if (this.notSelectedSkills.length > 0) {
      this._skillSelectionVms.push(new SkillSelectionVm(this.notSelectedSkills));
    }

    this._specializationSelectionVms = this._specializationSelectionVms.filter(s => s.selectedId != null).filter(s => this.selectedSpeciality.specializations.some(sp => sp.id == s.selectedId));
    if (this.notSelectedSpecializations.length > 0 && this._specializationSelectionVms.length == 0) {
      this._specializationSelectionVms.push(new SpecializationSelectionVm(this.notSelectedSpecializations));
    }
  }

  public selectSpecialization(specializationId: string): void {
    if (this._specializationSelectionVms.some(s => s.selectedId == specializationId)) {
      return;
    }

    var relatedSpeciality = this.awailableSpecialities.find(s => s.id == this.selectedId);
    if (!relatedSpeciality || !relatedSpeciality.specializations.some(s => s.id == specializationId)) {
      return;
    }

    var newSpecializationSelectionVm = new SpecializationSelectionVm(this.notSelectedSpecializations);
    this._specializationSelectionVms.push(newSpecializationSelectionVm);
    newSpecializationSelectionVm.selectedId = specializationId;
    this.normalize(this.awailableSpecialities);
  }

  public selectSkill(skillId: string): void {
    if (this._skillSelectionVms.some(s => s.selectedId == skillId)) {
      return;
    }

    var relatedSpeciality = this.awailableSpecialities.find(s => s.id == this.selectedId);
    if (!relatedSpeciality || !relatedSpeciality.skills.some(s => s.id == skillId)) {
      return;
    }

    var newSkillSelectionVm = new SkillSelectionVm(this.notSelectedSkills);
    this._skillSelectionVms.push(newSkillSelectionVm);
    newSkillSelectionVm.selectedId = skillId;
    this.normalize(this.awailableSpecialities);
  }
}

class Vm {
  private _skillsMatrix: SkillsMatrix = null;
  private _specialitySelectionVms: SpecialitySelectionVm[] = [];

  private get allSpecialities(): Speciality[] {
    return this._skillsMatrix.specialities;
  }

  private get notSelectedSpecialities(): Speciality[] {
    return this._skillsMatrix.specialities.filter(s => !this.specialitySelectionVms.some(svm => s.id == svm.selectedId));
  }

  public constructor(skillsMatrix: SkillsMatrix) {
    this._skillsMatrix = skillsMatrix;
    this.normalize();
  }

  public get specialitySelectionVms(): SpecialitySelectionVm[] {
    return this._specialitySelectionVms;
  }

  public reset(): void {

    for (let specialitySelectionVm of this._specialitySelectionVms) {
      specialitySelectionVm.selectedId = null;

      for (var specializationSelectionVm of specialitySelectionVm.specializationSelectionVms) {
        specializationSelectionVm.selectedId = null;
      }

      for (var skillSelectionVm of specialitySelectionVm.skillSelectionVms) {
        skillSelectionVm.selectedId = null;
      }
    }

    this.normalize();
  }

  public normalize(): void {
    this._specialitySelectionVms = this.specialitySelectionVms.filter(s => s.selectedId != null);

    for (var specialitySelectionVm of this.specialitySelectionVms) {
      var selectedSpeciality = this._skillsMatrix.specialities.find(s => s.id == specialitySelectionVm.selectedId);
      specialitySelectionVm.normalize([selectedSpeciality, ...this.notSelectedSpecialities]);
    }

    if (this.specialitySelectionVms.length < 1 && this.notSelectedSpecialities.length > 0) {
      this.specialitySelectionVms.push(new SpecialitySelectionVm(this.notSelectedSpecialities));
    }
  }

  public selectSpeciality(specialityId: string): void {
    if (this.specialitySelectionVms.some(s => s.selectedId == specialityId)) {
      return;
    }

    if (!this._skillsMatrix.specialities.some(s => s.id == specialityId)) {
      return;
    }

    var newSpecialitySelectionVms = new SpecialitySelectionVm(this.notSelectedSpecialities);
    this.specialitySelectionVms.push(newSpecialitySelectionVms);
    newSpecialitySelectionVms.selectedId = specialityId;
    this.normalize();
  }

  public selectSpecialization(specializationId: string): void {
    var relatedSpeciality = this._skillsMatrix.specialities.find(s => s.specializations.some(sp => sp.id == specializationId));

    if (!relatedSpeciality) {
      return;
    }

    this.selectSpeciality(relatedSpeciality.id);
    var relatedSpecialitySelectionVm = this.specialitySelectionVms.find(s => s.selectedId == relatedSpeciality.id);
    if (relatedSpecialitySelectionVm) {
      relatedSpecialitySelectionVm.selectSpecialization(specializationId);
    }
  }

  public selectSkill(skillId: string): void {
    var relatedSpeciality = this._skillsMatrix.specialities.find(s => s.skills.some(sp => sp.id == skillId));

    if (!relatedSpeciality) {
      return;
    }

    this.selectSpeciality(relatedSpeciality.id);
    var relatedSpecialitySelectionVm = this.specialitySelectionVms.find(s => s.selectedId == relatedSpeciality.id);
    if (relatedSpecialitySelectionVm) {
      relatedSpecialitySelectionVm.selectSkill(skillId);
    }
  }

  public applySpecialitiesSelectionData(specialitiesSelectionData: SpecialitiesSelectionData): void {
    if (!specialitiesSelectionData) {
      this.applySpecialitiesSelectionData(new SpecialitiesSelectionData());
    }

    this.reset();

    for (var specialityId of specialitiesSelectionData.selectedSpecialitiesIds) {
      this.selectSpeciality(specialityId);
    }
    for (var specializationId of specialitiesSelectionData.selectedSpecializationsIds) {
      this.selectSpecialization(specializationId);
    }
    for (var skillId of specialitiesSelectionData.selectedSkillsIds) {
      this.selectSkill(skillId);
    }
  }

  public createSpecialitiesSelectionData(): SpecialitiesSelectionData {
    var specialitiesSelectionData = new SpecialitiesSelectionData(
      this.specialitySelectionVms
        .map(s => s.selectedId)
        .filter(id => id != null),
      this.specialitySelectionVms
        .map(s => s.specializationSelectionVms.map(sp => sp.selectedId))
        .reduce((res, x) => [...res, ...x], [])
        .filter(id => id != null),
      this.specialitySelectionVms
        .map(s => s.skillSelectionVms.map(sk => sk.selectedId))
        .reduce((res, x) => [...res, ...x], [])
        .filter(id => id != null),
    );
    return specialitiesSelectionData;
  }
}

@Component({
  selector: 'sk-specialities-selections',
  templateUrl: './sk-specialities-selections.component.html',
  styleUrls: ['./sk-specialities-selections.component.scss']
})
export class SkSpecialitiesSelectionsComponent implements OnInit, OnChanges {
  @Input() specialitiesSelectionData: SpecialitiesSelectionData = null;
  @Output() specialitiesSelectionDataChange: EventEmitter<SpecialitiesSelectionData> = new EventEmitter<SpecialitiesSelectionData>();

  private _skillsMatrixDirectory: SkillsMatrixDirectory;

  private _vm: Vm = null;
  private _skillsMatrix: SkillsMatrix = null;
  private _specialitiesSelectionData: SpecialitiesSelectionData = null;

  public constructor(skillsMatrixDirectory: SkillsMatrixDirectory) {
    this._skillsMatrixDirectory = skillsMatrixDirectory;
  }

  public get vm(): Vm {
    return this._vm;
  }

  public async ngOnInit(): Promise<void> {
    this._skillsMatrix = await this._skillsMatrixDirectory.get();
    this._vm = new Vm(this._skillsMatrix);

    if (this.specialitiesSelectionData) {
      this._vm.applySpecialitiesSelectionData(this.specialitiesSelectionData);
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes["specialitiesSelectionData"] && this._vm) {
      var isChanged = JSON.stringify(changes["specialitiesSelectionData"].currentValue) != JSON.stringify(changes["specialitiesSelectionData"].previousValue);
      if (isChanged) {
        this._vm.applySpecialitiesSelectionData(this.specialitiesSelectionData);
      }
    }
  }

  public onSomethingChanged(): void {
    this._vm.normalize();
    this.specialitiesSelectionDataChange.emit(this._vm.createSpecialitiesSelectionData());
  }
}
