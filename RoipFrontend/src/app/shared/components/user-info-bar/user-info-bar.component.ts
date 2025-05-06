// user-info-bar.component.ts
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserModel } from '../../../models/user.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-user-info-bar',
  templateUrl: './user-info-bar.component.html',
  styleUrls: ['./user-info-bar.component.scss']
})
export class UserInfoBarComponent implements OnInit, OnDestroy {
  user: UserModel | null = null;
  routeDataSubscription: Subscription | undefined;

  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.routeDataSubscription = this.route.data.subscribe(data => {
      this.user = data['userData'];
    });
  }

  ngOnDestroy(): void {
    if (this.routeDataSubscription) {
      this.routeDataSubscription.unsubscribe();
    }
  }
}
