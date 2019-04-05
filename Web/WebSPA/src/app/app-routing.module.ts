import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent} from './home/home.component'
import { CashdeskComponent } from './cashdesk/cashdesk.component';
import { AuthenticationComponent } from '../app/authentication/authentication.component'

const routes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'cashdesk', component: CashdeskComponent },
  { path: 'authentication', component: AuthenticationComponent },
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
