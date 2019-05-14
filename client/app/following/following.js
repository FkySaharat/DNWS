'use strict';

angular.module('followingList', ['ngRoute'])
  .component('followingList', {
    templateUrl: 'following/following.html',
    controller: ['$http', '$rootScope', function TweetListController($http, $rootScope) {
      var self = this;

      const requestOptions = {
          headers: { 'X-session': $rootScope.x_session }
      };

      $http.get('http://localhost:8080/twitterapi/following/', requestOptions).then(function (response) {
        self.followings = response.data;
      });

      self.sendfollowing = function sendfollowing(username)
      {
        const data = "followingname=" + encodeURIComponent(username);
        $http.post('http://localhost:8080/twitterapi/following/', data, requestOptions).then(function (response) {
            
        });
      }
      self.unfollowing = function unfollowing(username) {
            const data = "followingname=" + encodeURIComponent(username);
            $http.delete('http://localhost:8080/twitterapi/following/?'+ data, requestOptions).then(function (response) {

            });
      }

    }]
});