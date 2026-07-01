import { Component, OnInit } from '@angular/core';
import { ShoppingBuddyService } from '../../../core/services/shopping-buddy.service';

@Component({
  selector: 'app-safety-legal',
  templateUrl: './safety-legal.component.html',
  styleUrls: ['./safety-legal.component.css'],
})
export class SafetyLegalComponent implements OnInit {
  safetyRules: string[] = [];
  verificationSteps: { step: number; title: string; detail: string }[] = [];

  constructor(private readonly buddyService: ShoppingBuddyService) {}

  ngOnInit(): void {
    this.safetyRules = this.buddyService.getSafetyRules();
    this.verificationSteps = this.buddyService.getVerificationSteps();
  }
}
