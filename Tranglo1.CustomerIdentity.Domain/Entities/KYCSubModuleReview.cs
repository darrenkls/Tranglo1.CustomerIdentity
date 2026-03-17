using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class KYCSubModuleReview
    {
        public KYCCategory KYCCategory { get; set; }
        public ReviewResult ReviewResult { get; set; }
        public DateTime? UserUpdateDate { get; private set; }
        public DateTime? LastReviewedDate { get; set; }
        public BusinessProfile BusinessProfile { get; set; }

        public int BusinessProfileCode { get; private set; }
        public long KYCCategoryCode { get; set; }

        private KYCSubModuleReview()
        {

        }

        public KYCSubModuleReview(BusinessProfile businessProfile, KYCCategory kYCCategory, ReviewResult reviewResult )
        {
            this.BusinessProfile = businessProfile;
            this.KYCCategory = kYCCategory;
            this.ReviewResult = reviewResult;
        }

        public void UpdateUserUpdatedDate()
        {
            this.UserUpdateDate = DateTime.UtcNow;
        }

        public void UpdateLastReviewDate()
        {
            this.LastReviewedDate = DateTime.UtcNow;
        }

        public void AssignReviewResult(ReviewResult reviewResult)
        {
            if (this.ReviewResult != reviewResult)
            {
                this.ReviewResult = reviewResult;
            }
        }
    }
}
