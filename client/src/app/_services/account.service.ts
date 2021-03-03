import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators'; 
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  curentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  login(model:any)
  {
    return this.http.post(this.baseUrl + 'account/login',model).pipe(
      map((response:User) =>
      {
        const user = response;
        if(user)
        {
          this.setCurrentUser(user);
        }
      })
    );
  }

  logout()
  {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  setCurrentUser(user:User)
  {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role; //gets the role from the token
    Array.isArray(roles) ? user.roles=roles : user.roles.push(roles);
    localStorage.setItem('user',JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  register(model:any)
  {
    return this.http.post(this.baseUrl + "account/register", model).pipe(
      map((user:User) =>{
        if(user)
        {
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }

  getDecodedToken(token){
    //atob let's us see what it is in the token
    //token has 3 sections and we want the payload section, that's why is passed [1] and using split
    return JSON.parse(atob(token.split('.')[1]));
  }
}
