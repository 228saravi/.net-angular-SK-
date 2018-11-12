import { NgModule, APP_INITIALIZER } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { ShareButtonModule } from '@ngx-share/button';

import { createChatInitializer, Chat } from '../_services/sk-domain-chat.service';
import { createSecurityInitializer, Security } from '../_services/sk-security.service';

import { SkVacanciesSearchPageComponent } from './sk-vacancies-search-page/sk-vacancies-search-page.component';
import { SkNavComponent } from './sk-nav/sk-nav.component';
import { SkVacanciesFilterComponent } from './sk-vacancies-filter/sk-vacancies-filter.component';
import { SkUiKitModule } from '../sk-ui-kit/sk-ui-kit.module';
import { SkSpecialitiesSelectionsComponent } from './sk-specialities-selections/sk-specialities-selections.component';
import { SkVacancyDetailsPageComponent } from './sk-vacancy-details-page/sk-vacancy-details-page.component';
import { SkSignInPopupComponent } from './sk-sign-in-popup/sk-sign-in-popup.component';
import { SkExpertsSearchPageComponent } from './sk-experts-search-page/sk-experts-search-page.component';
import { SkLoginPageComponent } from './sk-login-page/sk-login-page.component';
import { SkTimePipe } from './sk-time.pipe';
import { SkDatePipe } from './sk-date.pipe';
import { SkSpecialityIconComponent } from './sk-speciality-icon/sk-speciality-icon.component';
import { SkConnectionsPageComponent } from './sk-connections-page/sk-connections-page.component';
import { SkDialogsPageComponent } from './sk-dialogs-page/sk-dialogs-page.component';
import { SkExpertProfileDetailsPageComponent } from './sk-expert-profile-details-page/sk-expert-profile-details-page.component';
import { SkFormattedTextComponent } from './sk-formatted-text/sk-formatted-text.component';
import { SkEventDetailsPageComponent } from './sk-event-details-page/sk-event-details-page.component';
import { SkCompanyDetailsPageComponent } from './sk-company-details-page/sk-company-details-page.component';
import { SkExpertsFilterComponent } from './sk-experts-filter/sk-experts-filter.component';
import { SkErrorPageComponent } from './sk-error-page/sk-error-page.component';
import { SkNumberPipe } from './sk-number.pipe';
import { SkRatingComponent } from './sk-rating/sk-rating.component';
import { SkPublishCheckListPopupComponent } from './sk-publish-check-list-popup/sk-publish-check-list-popup.component';
import { SkConnectCompanyToExpertPopupComponent } from './sk-connect-company-to-expert-popup/sk-connect-company-to-expert-popup.component';
import { SkExpertConnectionsPageComponent } from './sk-expert-connections-page/sk-expert-connections-page.component';
import { SkCompanyConnectionsPageComponent } from './sk-company-connections-page/sk-company-connections-page.component';
import { SkFeedbackPopupComponent } from './sk-feedback-popup/sk-feedback-popup.component';
import { SkCompanyDialogsComponent } from './sk-company-dialogs/sk-company-dialogs.component';
import { SkExpertDialogsComponent } from './sk-expert-dialogs/sk-expert-dialogs.component';
import { SkHomePageComponent } from './sk-home-page/sk-home-page.component';
import { SkPublishBeforeConnectPopupComponent } from './sk-publish-before-connect-popup/sk-publish-before-connect-popup.component';

// Эти импорты нужны для кнопок "поделиться"
import { library } from '@fortawesome/fontawesome-svg-core';

import { faFacebookF } from '@fortawesome/free-brands-svg-icons/faFacebookF';
import { faTwitter } from '@fortawesome/free-brands-svg-icons/faTwitter';
import { faRedditAlien } from '@fortawesome/free-brands-svg-icons/faRedditAlien';
import { faLinkedinIn } from '@fortawesome/free-brands-svg-icons/faLinkedinIn';
import { faGooglePlusG } from '@fortawesome/free-brands-svg-icons/faGooglePlusG';
import { faTumblr } from '@fortawesome/free-brands-svg-icons/faTumblr';
import { faPinterestP } from '@fortawesome/free-brands-svg-icons/faPinterestP';
import { faWhatsapp } from '@fortawesome/free-brands-svg-icons/faWhatsapp';
import { faVk } from '@fortawesome/free-brands-svg-icons/faVk';
import { faFacebookMessenger } from '@fortawesome/free-brands-svg-icons/faFacebookMessenger';
import { faTelegramPlane } from '@fortawesome/free-brands-svg-icons/faTelegramPlane';
import { faStumbleupon } from '@fortawesome/free-brands-svg-icons/faStumbleupon';
import { faXing } from '@fortawesome/free-brands-svg-icons/faXing';

import { faCommentAlt } from '@fortawesome/free-solid-svg-icons/faCommentAlt';
import { faMinus } from '@fortawesome/free-solid-svg-icons/faMinus';
import { faEllipsisH } from '@fortawesome/free-solid-svg-icons/faEllipsisH';
import { faLink } from '@fortawesome/free-solid-svg-icons/faLink';
import { faExclamation } from '@fortawesome/free-solid-svg-icons/faExclamation';
import { faPrint } from '@fortawesome/free-solid-svg-icons/faPrint';
import { faCheck } from '@fortawesome/free-solid-svg-icons/faCheck';
import { faEnvelope } from '@fortawesome/free-solid-svg-icons/faEnvelope';
import { SkTestPageComponent } from './sk-test-page/sk-test-page.component';

// Регистрация иконок. Нужно для кнопок "поделиться".
const icons = [
  faFacebookF, faTwitter, faLinkedinIn, faGooglePlusG, faPinterestP, faRedditAlien, faTumblr,
  faWhatsapp, faVk, faFacebookMessenger, faTelegramPlane, faStumbleupon, faXing, faCommentAlt,
  faEnvelope, faCheck, faPrint, faExclamation, faLink, faEllipsisH, faMinus
];

library.add(...icons);


@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    RouterModule,
    SkUiKitModule,
    FormsModule,
    ShareButtonModule.forRoot(),
  ],
  providers: [
    { provide: APP_INITIALIZER, useFactory: createSecurityInitializer, deps: [Security], multi: true },
    { provide: APP_INITIALIZER, useFactory: createChatInitializer, deps: [Chat], multi: true },
  ],
  declarations: [
    SkVacanciesSearchPageComponent,
    SkNavComponent,
    SkVacanciesFilterComponent,
    SkSpecialitiesSelectionsComponent,
    SkVacancyDetailsPageComponent,
    SkSignInPopupComponent,
    SkExpertsSearchPageComponent,
    SkLoginPageComponent,
    SkTimePipe,
    SkDatePipe,
    SkNumberPipe,
    SkSpecialityIconComponent,
    SkConnectionsPageComponent,
    SkDialogsPageComponent,
    SkExpertProfileDetailsPageComponent,
    SkFormattedTextComponent,
    SkEventDetailsPageComponent,
    SkCompanyDetailsPageComponent,
    SkExpertsFilterComponent,
    SkErrorPageComponent,
    SkPublishCheckListPopupComponent,
    SkRatingComponent,
    SkPublishCheckListPopupComponent,
    SkConnectCompanyToExpertPopupComponent,
    SkExpertConnectionsPageComponent,
    SkCompanyConnectionsPageComponent,
    SkFeedbackPopupComponent,
    SkCompanyDialogsComponent,
    SkExpertDialogsComponent,
    SkTestPageComponent,
    SkHomePageComponent,
    SkPublishBeforeConnectPopupComponent,
  ],
  exports: [
    SkSignInPopupComponent,
    SkPublishCheckListPopupComponent,
  ]
})
export class SkCommonModule { }
