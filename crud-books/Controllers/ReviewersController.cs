using System.Collections.Generic;
using System.Linq;
using BookApiProject.Dtos;
using BookApiProject.Models;
using BookApiProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewersController: Controller
    {
        private IReviewerRepository _reviewerRepository;
        private IReviewRepository _reviewRepository;

        public ReviewersController(IReviewerRepository reviewerRepository, IReviewRepository reviewRepository)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
        }

        //api/reviewers
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        [ProducesResponseType(400)]
        public ActionResult GetReviewers()
        {
            var reviewers = _reviewerRepository.GetReviewers();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new List<ReviewerDto>();

            foreach (var reviewer in reviewers)
            {
                reviewerDto.Add(new ReviewerDto
                {
                    Id = reviewer.Id,
                    FirstName = reviewer.FirstName,
                    LastName = reviewer.LastName
                });
            }
            
            return Ok(reviewerDto);
        }

        //api/reviewers/reviewerId
        [HttpGet("{reviewerId}", Name = "GetReviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewer (int reviewerId)
        {
            if(!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviewer = _reviewerRepository.GetReviewer(reviewerId);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewersDto = new ReviewerDto()
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewersDto);
        }


        //api/reviewers/reviewerId/reviews
        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if(!_reviewerRepository.ReviewerExists(reviewerId))
                    return NotFound();
            
            var reviewer = _reviewerRepository.GetReviewsByReviewer(reviewerId);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();
            foreach (var review in reviewer)
            {
                reviewsDto.Add(new ReviewDto()
                {
                    Id = review.Id,
                    Headline = review.Headline,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                });
            }

            return Ok(reviewsDto);
        }


        //api/reviewers/reviewId/reviewer
        [HttpGet("{reviewId}/reviewer")]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewerOfAReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var reviewer = _reviewerRepository.GetReviewerOfAReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto()
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewerDto);
        }

         //api/reviewers
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateReviewer([FromBody]Reviewer reviewerToCreate)
        {
            if (reviewerToCreate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.CreateReviewer(reviewerToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving " +
                                            $"{reviewerToCreate.FirstName} {reviewerToCreate.LastName}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetReviewer", new { reviewerId = reviewerToCreate.Id }, reviewerToCreate);
        }

        //api/reviewers/reviewId
        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)] 
        [ProducesResponseType(500)]        
        public IActionResult UpdateReviewer(int reviewId, [FromBody]Reviewer updatedReviewerInfo)
        {
            if (updatedReviewerInfo == null)
                return BadRequest(ModelState);

            if (reviewId != updatedReviewerInfo.Id)
                return BadRequest(ModelState);

            if (!_reviewerRepository.ReviewerExists(reviewId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

             if (!_reviewerRepository.UpdateReviewer(updatedReviewerInfo))
            {
                ModelState.AddModelError("", $"Something went wrong updating " +
                                            $"{updatedReviewerInfo.FirstName} {updatedReviewerInfo.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        //api/reviewers/reviewerId
        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);
            var reviewsToDelete = _reviewerRepository.GetReviewsByReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting " +
                                            $"{reviewerToDelete.FirstName} {reviewerToDelete.LastName}");
                return StatusCode(500, ModelState);
            }

            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", $"Something went wrong deleting reviews by" +
                                            $"{reviewerToDelete.FirstName} {reviewerToDelete.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


    }
}