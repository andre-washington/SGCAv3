import { Component, OnInit } from '@angular/core';
import { SignalRService } from './services/signal-r.service';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(public signalRService: SignalRService, private http: HttpClient) { }

  ngOnInit() {
    this.signalRService.startConnection();
    this.signalRService.addTransferChartDataListener();  //captura o evento 
    this.startHttpRequest();
    }
  private startHttpRequest = () => {
    this.http.get('http://localhost:5000/api/caixa')
      .subscribe(res => {
        console.log("testeappcomponents");
      })
  }

  onNavigate(){
    window.open("saque", "_blank"); //modificar
}
  title = 'sgceui';
}
