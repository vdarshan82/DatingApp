import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

const httpOptions = {
  headers: new HttpHeaders({
    'Authorization': 'Bearer ' + localStorage.getItem('token')
  })
}

@Injectable({
  providedIn: 'root'
})
export class UserService {

baseUrl: string = environment.apiUrl;
constructor(private http: HttpClient) { }

getUsers(): Observable<User[]> {
  console.log('Inside getUsers service');

 return this.http.get<User[]>(this.baseUrl + 'user', httpOptions);

}

getUser(id): Observable<User> {
  return this.http.get<User>(this.baseUrl + 'user/' + id, httpOptions);
 }

}


