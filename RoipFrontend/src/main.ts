import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';
import { Router, RouterModule } from '@angular/router';


console.log('Starting the Angular application...');
// Bootstrap the Angular application using the AppModule
platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error('Error bootstrapping the application:', err));

