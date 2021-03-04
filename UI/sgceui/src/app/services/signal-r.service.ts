import { Injectable } from '@angular/core';
import * as signalR from "@aspnet/signalr";
import { HttpClient } from '@angular/common/http';
import { SharedService} from 'src/app/shared.service';



@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  public  CaixaSignalList: any[]=[];
  public  SaqueSignalStatus: any[]=[];
  constructor(private http:HttpClient, private service:SharedService) { }

  private hubConnection: signalR.HubConnection;
  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
                            .configureLogging(signalR.LogLevel.Debug)
                            .withUrl('http://localhost:5000/signalr'
                            ,{
                              skipNegotiation: true,
                              transport: signalR.HttpTransportType.WebSockets
                            })
                            .build();
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err))
  }
  public addTransferChartDataListener = () => {
    this.hubConnection.on('transfercaixadata', (data) => {
      this.CaixaSignalList = data.value; //resultado do hub
      
      this.startHttpRequest();
      console.log(data.value); //este exibe
    } );
  }
  public addSaqueStatusListener = () => {
    this.hubConnection.on('caixaativoevent', (data) => {
      this.SaqueSignalStatus = data.value; //resultado do hub
      
      this.startHttpRequest();
      console.log(data.value); //este exibe
    } );
  }
  private startHttpRequest = () => {
    this.http.get('http://localhost:5000/api/caixa')
      .subscribe(res => {
        this.service.getCaixaSignalStatus().subscribe(
          data => {
          this.CaixaSignalList=data; //aqui vem do get
        });
      })
  }
}
