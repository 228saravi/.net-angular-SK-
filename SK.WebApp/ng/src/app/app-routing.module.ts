import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { SkErrorPageComponent } from "./sk-common/sk-error-page/sk-error-page.component";
import { SkVacanciesSearchPageComponent } from "./sk-common/sk-vacancies-search-page/sk-vacancies-search-page.component";
import { SkVacancyDetailsPageComponent } from "./sk-common/sk-vacancy-details-page/sk-vacancy-details-page.component";
import { AuthGuard } from "./sk-common/sk-auth.guard";
import { SkExpertsSearchPageComponent } from "./sk-common/sk-experts-search-page/sk-experts-search-page.component";
import { SkLoginPageComponent } from "./sk-common/sk-login-page/sk-login-page.component";
import { SkConnectionsPageComponent } from "./sk-common/sk-connections-page/sk-connections-page.component";
import { SkDialogsPageComponent } from "./sk-common/sk-dialogs-page/sk-dialogs-page.component";
import { SkExpertProfileDetailsPageComponent } from "./sk-common/sk-expert-profile-details-page/sk-expert-profile-details-page.component";
import { SkEventDetailsPageComponent } from "./sk-common/sk-event-details-page/sk-event-details-page.component";
import { SkCompanyDetailsPageComponent } from "./sk-common/sk-company-details-page/sk-company-details-page.component";
import { SkHomePageComponent } from "./sk-common/sk-home-page/sk-home-page.component";
import { SkTestPageComponent } from "./sk-common/sk-test-page/sk-test-page.component";


const appRoutes: Routes = [
  { path: 'error-page', component: SkErrorPageComponent },
  { path: 'test', component: SkTestPageComponent },

  { path: 'vacancies', component: SkVacanciesSearchPageComponent },
  { path: 'vacancies/:id', component: SkVacancyDetailsPageComponent },

  { path: 'events/:id', component: SkEventDetailsPageComponent },

  { path: 'companies/:id', component: SkCompanyDetailsPageComponent },

  { path: 'experts', component: SkExpertsSearchPageComponent, canActivate: [AuthGuard] },
  { path: 'experts/:id', component: SkExpertProfileDetailsPageComponent, canActivate: [AuthGuard] },

  { path: 'connections', component: SkConnectionsPageComponent, canActivate: [AuthGuard] },
  { path: 'dialogs', component: SkDialogsPageComponent, canActivate: [AuthGuard] },
  { path: 'login', component: SkLoginPageComponent },
  { path: '', component: SkHomePageComponent },
];

@NgModule({
  imports: [
    RouterModule.forRoot(
      appRoutes,
      //{
      //  enableTracing: true,
      //  onSameUrlNavigation: 'reload'
      //} // <-- debugging purposes only
    )
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule { }
