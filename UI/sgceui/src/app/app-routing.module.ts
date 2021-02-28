import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import {CaixaComponent} from './caixa/caixa.component';


const routes: Routes = [
{path:'caixa', component:CaixaComponent}

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
