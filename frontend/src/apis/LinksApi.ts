/* tslint:disable */
/* eslint-disable */
/**
 * Api | v1
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0.0
 *
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

import * as runtime from './runtime';
import type { Link, LinkTag, PagedResultsOfLinkItem, ProblemDetails } from '../models/index';
import {
   LinkFromJSON,
   LinkToJSON,
   LinkTagFromJSON,
   LinkTagToJSON,
   PagedResultsOfLinkItemFromJSON,
   PagedResultsOfLinkItemToJSON,
   ProblemDetailsFromJSON,
   ProblemDetailsToJSON,
} from '../models/index';

export interface ApiV1LinksGetRequest {
   term?: string;
   domain?: string;
   tags?: string;
   pageNo?: number;
   pageSize?: number;
}

export interface ApiV1LinksLinkIdGetRequest {
   linkId: string;
}

export interface ApiV1LinksTagsGetRequest {
   tags?: string;
   count?: number;
}

/**
 *
 */
export class LinksApi extends runtime.BaseAPI {
   /**
    */
   async apiV1LinksGetRaw(
      requestParameters: ApiV1LinksGetRequest,
      initOverrides?: RequestInit | runtime.InitOverrideFunction,
   ): Promise<runtime.ApiResponse<PagedResultsOfLinkItem>> {
      const queryParameters: any = {};

      if (requestParameters['term'] != null) {
         queryParameters['term'] = requestParameters['term'];
      }

      if (requestParameters['domain'] != null) {
         queryParameters['domain'] = requestParameters['domain'];
      }

      if (requestParameters['tags'] != null) {
         queryParameters['tags'] = requestParameters['tags'];
      }

      if (requestParameters['pageNo'] != null) {
         queryParameters['pageNo'] = requestParameters['pageNo'];
      }

      if (requestParameters['pageSize'] != null) {
         queryParameters['pageSize'] = requestParameters['pageSize'];
      }

      const headerParameters: runtime.HTTPHeaders = {};

      const response = await this.request(
         {
            path: `/api/v1/links`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
         },
         initOverrides,
      );

      return new runtime.JSONApiResponse(response, (jsonValue) =>
         PagedResultsOfLinkItemFromJSON(jsonValue),
      );
   }

   /**
    */
   async apiV1LinksGet(
      requestParameters: ApiV1LinksGetRequest = {},
      initOverrides?: RequestInit | runtime.InitOverrideFunction,
   ): Promise<PagedResultsOfLinkItem> {
      const response = await this.apiV1LinksGetRaw(requestParameters, initOverrides);
      return await response.value();
   }

   /**
    * Gets a single link item
    */
   async apiV1LinksLinkIdGetRaw(
      requestParameters: ApiV1LinksLinkIdGetRequest,
      initOverrides?: RequestInit | runtime.InitOverrideFunction,
   ): Promise<runtime.ApiResponse<Link>> {
      if (requestParameters['linkId'] == null) {
         throw new runtime.RequiredError(
            'linkId',
            'Required parameter "linkId" was null or undefined when calling apiV1LinksLinkIdGet().',
         );
      }

      const queryParameters: any = {};

      const headerParameters: runtime.HTTPHeaders = {};

      const response = await this.request(
         {
            path: `/api/v1/links/{linkId}`.replace(
               `{${'linkId'}}`,
               encodeURIComponent(String(requestParameters['linkId'])),
            ),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
         },
         initOverrides,
      );

      return new runtime.JSONApiResponse(response, (jsonValue) => LinkFromJSON(jsonValue));
   }

   /**
    * Gets a single link item
    */
   async apiV1LinksLinkIdGet(
      requestParameters: ApiV1LinksLinkIdGetRequest,
      initOverrides?: RequestInit | runtime.InitOverrideFunction,
   ): Promise<Link> {
      const response = await this.apiV1LinksLinkIdGetRaw(requestParameters, initOverrides);
      return await response.value();
   }

   /**
    */
   async apiV1LinksTagsGetRaw(
      requestParameters: ApiV1LinksTagsGetRequest,
      initOverrides?: RequestInit | runtime.InitOverrideFunction,
   ): Promise<runtime.ApiResponse<Array<LinkTag>>> {
      const queryParameters: any = {};

      if (requestParameters['tags'] != null) {
         queryParameters['tags'] = requestParameters['tags'];
      }

      if (requestParameters['count'] != null) {
         queryParameters['count'] = requestParameters['count'];
      }

      const headerParameters: runtime.HTTPHeaders = {};

      const response = await this.request(
         {
            path: `/api/v1/links/tags`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
         },
         initOverrides,
      );

      return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(LinkTagFromJSON));
   }

   /**
    */
   async apiV1LinksTagsGet(
      requestParameters: ApiV1LinksTagsGetRequest = {},
      initOverrides?: RequestInit | runtime.InitOverrideFunction,
   ): Promise<Array<LinkTag>> {
      const response = await this.apiV1LinksTagsGetRaw(requestParameters, initOverrides);
      return await response.value();
   }
}
