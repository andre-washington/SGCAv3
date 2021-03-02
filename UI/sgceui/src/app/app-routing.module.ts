import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import {CaixaComponent} from './caixa/caixa.component';
import {SaqueComponent} from './saque/saque.component';



const routes: Routes = [
{path:'caixa', component:CaixaComponent},
{path:'saque', component:SaqueComponent}

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
